// #define RESET_ON_REGISTER // when true, clears the server database every time a host registers (don't use for production)
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using JsonFx.Json;

public class NetManager : SocketIOComponent {

	struct ConnectionInfo {

		public string name;
		public string clientId;
		public Response.Room room;
		public string roomId { get { return connected ? room._id : ""; } }
		public bool connected { get { return room != null; } }

		public void Reset () {
			name = "";
			clientId = "";
			room = null;
		}

		public override string ToString () {
			string output = "";
			if (!string.IsNullOrEmpty (name))
				output += "name: " + name;
			if (!string.IsNullOrEmpty (clientId))
				output += ", clientId: " + clientId;
			if (connected)
				output += ", roomId: " + roomId;
			return output;
		}
	}

	public delegate void ClientsUpdated (string[] clients);
	public delegate void OnDisconnected ();
	public delegate void MessageReceived (NetMessage msg);
	public delegate void OnUpdateConnection (bool connected);
	public delegate void OnUpdateDroppedClients (bool hasDroppedClients);
	public delegate void OnSocketDisconnected ();

	public ClientsUpdated clientsUpdated;
	public OnDisconnected onDisconnected;
	public MessageReceived messageReceived;
	public OnUpdateConnection onUpdateConnection;
	public OnUpdateDroppedClients onUpdateDroppedClients;
	public OnSocketDisconnected onSocketDisconnected;

	ConnectionInfo connection;
	Action<Dictionary<string, string>> roomListResult;

	#if UNITY_EDITOR && SINGLE_SCREEN
	public string RoomId {
		get { return connection.roomId; }
	}
	#endif

	bool dropped = false;
	bool hasDroppedClients = false;

	public void Init () {
		On("open", OnOpen);
		On("error", OnError);
		On("close", OnClose);
		On("receiveMessage", OnMessage);
		Connect ();
	}

	/**
	 *	Host methods
	 */

	public void StartAsHost (string name, Action<ResponseType> response) {

		#if RESET_ON_REGISTER
		Emit ("socketReset", (SocketIOEvent e) => {
		#endif
			
		// Register and create the room
		connection.name = name;
		JSONObject obj = JSONObject.Create ();
		obj.AddField ("name", name);
		obj.AddField ("maxClientCount", 4);
		
		Emit<Response.CreateRoom> ("createRoom", obj, (Response.CreateRoom res) => {

			// If someone else already has this name, don't continue
			if (!res.nameTaken) {
				
				// Listen for clients joining the room
				On("updateClients", (SocketIOEvent ev) => {
					if (clientsUpdated != null)
						clientsUpdated (ev.Deserialize<Response.ClientList> ().ClientNames);
				});

				// Update ConnectionInfo
				Register (res.client._id, res.room);
			}

			response (res.nameTaken ? ResponseType.NameTaken : ResponseType.Success);
		});

		#if RESET_ON_REGISTER
		});
		#endif
	}

	public void CloseRoom () {

		// The room is closed when the game begins so that other players will not be able to join an in-progress game
		Emit ("closeRoom", connection.roomId);
	}

	/**
	 *	Client methods
	 */

	public void RequestRoomList (Action<Dictionary<string, string>> response) {

		On("roomListUpdated", OnUpdateRoomList);
		roomListResult = response;

		Emit<Response.RoomList> ("requestRoomList", (Response.RoomList res) => {
			response(res.ToDictionary ());
		});
	}

	public void StartAsClient (string name, string roomId, Action<ResponseType> response) {

		// Register the client
		connection.name = name;
		JSONObject obj = JSONObject.Create ();
		obj.AddField ("name", name);
		obj.AddField ("roomId", roomId);

		// Request to join the room
		Emit<Response.JoinRoom> ("joinRoom", obj, (Response.JoinRoom res) => {

			// If someone else already has this name, don't continue
			if (!res.nameTaken) {

				// Stop searching for rooms to join and listen for if this room is shut down
				Off ("roomListUpdated", OnUpdateRoomList);
				On ("kick", OnRoomDestroyed);

				// Update ConnectionInfo
				Register (res.client._id, res.room);
			}

			response (res.nameTaken ? ResponseType.NameTaken : ResponseType.Success);
		});
	}

	/**
	 *	Client & Host methods
	 */

	public void SendMessage (NetMessage msg) {

		JSONObject obj;

		if (msg.id == "InstanceDataLoaded") { // Special case
			obj = new JSONObject (msg.str1);
			obj.AddField ("roomId", connection.roomId);
			obj.AddField ("key", msg.id);
		} else {
			obj = JSONObject.Create ();
			obj.AddField ("roomId", connection.roomId);
			obj.AddField ("key", msg.id);
			obj.AddField ("str1", msg.str1);
			obj.AddField ("str2", msg.str2);
			obj.AddField ("val", msg.val);
		}

		Emit ("sendMessage", obj);
	}

	public void Stop () {

		if (connection.clientId == null)
			return;

		// Unregisters the client and leaves the room (if in one)
		JSONObject obj = JSONObject.Create ();
		obj.AddField ("roomId", connection.connected ? connection.roomId : "");
		obj.AddField ("clientId", connection.clientId);

		Emit ("leaveRoom", obj, (SocketIOEvent e) => {
			OnRoomDestroyed ();
		});
	}

	public void OnLoseFocus () { dropped = true; }

	public void OnGainFocus () { if (dropped) Close (); }

	// Simulate a dropped connection
	public void Drop () {
		dropped = true;
		Close ();
	}

	// Simulate reconnecting after a dropped connection
	public void Reconnect () {
		if (dropped) {
			Debug.Log ("reconnecting...");
			Connect ();
		}
	}

	/**
	 *	Private methods
	 */

	void Register (string clientId, Response.Room room) {

		// Add room data to ConnectionInfo
		connection.clientId = clientId;
		connection.room = room;

		// Listen for dropped clients
		Co.InvokeWhileTrue (0.5f, () => { return Application.isPlaying && connection.connected; }, () => {

			Emit<Response.DroppedClients> ("checkDropped", connection.roomId, (Response.DroppedClients res) => {

				// Ignore if this client has been dropped
				if (dropped)
					return;

				// Send a message if a client was dropped or if previously dropped clients have reconnected
				if (res.dropped && !hasDroppedClients) {

					if (onUpdateDroppedClients != null)
						onUpdateDroppedClients (true);

					hasDroppedClients = true;
					
				} else if (!res.dropped && hasDroppedClients) {

					if (onUpdateDroppedClients != null)
						onUpdateDroppedClients (false);

					hasDroppedClients = false;
				}

			});
		});
	}

	void SendUpdateConnectionMessage (bool connected) {
		if (onUpdateConnection != null)
			onUpdateConnection (connected);
	}

	/**
	 *	Events
	 */

	void OnMessage (SocketIOEvent e) {

		Response.Message msg = e.Deserialize<Response.Message> ();

		if (msg.key == "InstanceDataLoaded") { // Special case
			Models.InstanceData ins = e.Deserialize<Models.InstanceData> ();
			msg.str1 = JsonWriter.Serialize (ins);
		}

		if (messageReceived != null) {
			messageReceived (NetMessage.Create (msg.key, msg.str1, msg.str2, msg.val));
		}
	}

	void OnUpdateRoomList (SocketIOEvent e) {
		if (roomListResult != null) {
			Response.RoomList msg = e.Deserialize<Response.RoomList> ();
			roomListResult (msg.ToDictionary ());
		}
	}

	void OnRoomDestroyed (SocketIOEvent e=null) {
		connection.Reset ();
		if (onDisconnected != null)
			onDisconnected ();
	}

	void OnOpen (SocketIOEvent e=null) {
		
		SendUpdateConnectionMessage (true);

		if (IsConnected && dropped) {

			if (connection.connected) {
				JSONObject obj = JSONObject.Create ();
				obj.AddField ("clientId", connection.clientId);
				obj.AddField ("roomId", connection.roomId);

				Emit ("rejoinRoom", obj, (SocketIOEvent s) => {
					Debug.Log ("RECONNECTED!!!");
				});
			}

			dropped = false;
		}
	}

	void OnError (SocketIOEvent e) {
		#if UNITY_EDITOR
			Debug.LogWarning("[SocketIO] Error received: " + e.data);
		#endif

		if (e.data.ToString() == "An exception has occurred while connecting.") {
			SendUpdateConnectionMessage (false);
		}
		else if (e.data.ToString() == "An exception has occurred while receiving a message.") {
			onSocketDisconnected();
		}
	}

	// This event should only ever fire when the application is quit or when the device loses its connection (in which case it will attempt to reconnect)
	void OnClose (SocketIOEvent e) {
		#if !UNITY_EDITOR || !SINGLE_SCREEN
		Debug.Log ("got closed");
		SendUpdateConnectionMessage (false);
		Reconnect ();
		#endif
	}

	/**
	 *	Response models
	 */

	class Response {

		public class CreateRoom {
			public Room room { get; set; }
			public Client client { get; set; }
			public bool nameTaken { get; set; }
		}

		public class JoinRoom {
			public Room room { get; set; }
			public Client client { get; set; }
			public bool nameTaken { get; set; }
		}

		public class RoomList {

			public RoomListing[] rooms { get; set; }

			public Dictionary<string, string> ToDictionary () {
				Dictionary<string, string> r = new Dictionary<string, string> ();
				foreach (RoomListing room in rooms)
					r.Add (room.host, room.id);
				return r;
			}
		}

		public class ClientList {
			
			public Client[] clients { get; set; }

			public string[] ClientNames {
				get { return Array.ConvertAll (clients, x => x.name); }
			}
		}

		public class RoomListing {
			public string id { get; set; }
			public string host { get; set; }
		}

		public class Room {
			public string _id { get; set; }
			public Client host { get; set; }
			public Client[] clients { get; set; }
		}

		public class Client {
			public string _id { get; set; }
			public string name { get; set; }
		}

		public class Message {
			public string key { get; set; }
			public string str1 { get; set; }
			public string str2 { get; set; }
			public int val { get; set; }
		}

		public class DroppedClients {
			public bool dropped { get; set; }
		}
	}
}
