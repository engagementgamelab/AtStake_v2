#define RESET_ON_REGISTER
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using JsonFx.Json;

public class NetManager {

	struct ConnectionInfo {

		public string name;
		public string clientId;
		public Response.Room room;
		public string roomId { get { return room._id; } }
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

	public ClientsUpdated clientsUpdated;
	public OnDisconnected onDisconnected;
	public MessageReceived messageReceived;
	public OnUpdateConnection onUpdateConnection;

	ConnectionInfo connection;
	ConnectionInfo previousConnection;
	SocketIOComponent socket;
	Action<Dictionary<string, string>> roomListResult;

	#if UNITY_EDITOR && SINGLE_SCREEN
	public string RoomId {
		get { return connection.roomId; }
	}
	#endif

	bool dropped = false;

	public NetManager (SocketIOComponent socket) {
		this.socket = socket;
		this.socket.On("open", OnOpen);
		this.socket.On("error", OnError);
		this.socket.On("close", OnClose);
		this.socket.On("receiveMessage", OnMessage);
		this.socket.Connect ();
	}

	public void StartAsHost (string name, Action<ResponseType> response) {

		#if RESET_ON_REGISTER
		socket.Emit ("socketReset", (SocketIOEvent e) => {
		#endif
			
		connection.name = name;
		JSONObject obj = JSONObject.Create ();
		obj.AddField ("name", name);
		obj.AddField ("maxClientCount", 4);
		
		socket.Emit<Response.CreateRoom> ("createRoom", obj, (Response.CreateRoom res) => {
			if (!res.nameTaken) {
				socket.On("updateClients", OnUpdateClients);
				Register (res.client._id, res.room);
			}
			response (res.nameTaken ? ResponseType.NameTaken : ResponseType.Success);
		});

		#if RESET_ON_REGISTER
		});
		#endif
	}

	public void RequestRoomList (Action<Dictionary<string, string>> response) {

		socket.On("roomListUpdated", OnUpdateRoomList);
		roomListResult = response;

		socket.Emit<Response.RoomList> ("requestRoomList", (Response.RoomList res) => {
			response(res.ToDictionary ());
		});
	}

	public void StartAsClient (string name, string roomId, Action<ResponseType> response) {

		connection.name = name;
		JSONObject obj = JSONObject.Create ();
		obj.AddField ("name", name);
		obj.AddField ("roomId", roomId);

		socket.Emit<Response.JoinRoom> ("joinRoom", obj, (Response.JoinRoom res) => {
			if (!res.nameTaken) {
				socket.Off ("roomListUpdated", OnUpdateRoomList);
				socket.On ("kick", OnRoomClosed);
				Register (res.client._id, res.room);
			}
			response (res.nameTaken ? ResponseType.NameTaken : ResponseType.Success);
		});
	}

	public void CloseRoom () {

		// The room is closed when the game begins so that other players will not be able to join an in-progress game
		socket.Emit ("closeRoom", connection.roomId);
	}

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

		socket.Emit ("sendMessage", obj);
	}

	public void Stop () {

		if (connection.clientId == null)
			return;

		// Unregisters the client and leaves the room (if in one)
		JSONObject obj = JSONObject.Create ();
		obj.AddField ("roomId", connection.connected ? connection.roomId : "");
		obj.AddField ("clientId", connection.clientId);

		socket.Emit ("leaveRoom", obj, (SocketIOEvent e) => {
			connection.Reset ();
		});
	}

	// Simulate a dropped connection
	public void Drop () {
		dropped = true;
		socket.Close ();
	}

	void Register (string clientId, Response.Room room) {
		connection.clientId = clientId;
		connection.room = room;
		Co.InvokeWhileTrue (2f, () => { return Application.isPlaying && connection.connected; }, () => {
			socket.Emit<Response.DroppedClients> ("checkDropped", connection.roomId, (Response.DroppedClients res) => {
				if (res.dropped)
					OnRoomClosed ();
			});
		});
	}

	void OnUpdateClients (SocketIOEvent e) {
		if (clientsUpdated != null)
			clientsUpdated (e.Deserialize<Response.ClientList> ().ClientNames);
	}

	void OnMessage (SocketIOEvent e) {

		Response.Message msg = e.Deserialize<Response.Message> ();

		if (msg.key == "InstanceDataLoaded") { // Special case
			Models.InstanceData ins = e.Deserialize<Models.InstanceData> ();
			msg.str1 = JsonWriter.Serialize (ins);
		}

		if (messageReceived != null) {
			messageReceived (NetMessage.Create (msg.key, msg.str1, msg.str2, msg.val));
		}

		/*JSONObject obj = JSONObject.Create ();
		obj.AddField ("clientId", connection.clientId);
		obj.AddField ("roomId", connection.roomId);
		obj.AddField ("key", msg.key);

		socket.Emit ("confirmReceipt", obj);*/
	}

	void OnUpdateRoomList (SocketIOEvent e) {
		if (roomListResult != null) {
			Response.RoomList msg = e.Deserialize<Response.RoomList> ();
			roomListResult (msg.ToDictionary ());
		}
	}

	void OnRoomClosed (SocketIOEvent e=null) {
		previousConnection = connection;
		connection.Reset ();
		if (onDisconnected != null)
			onDisconnected ();
	}

	void OnOpen (SocketIOEvent e) {
		SendUpdateConnectionMessage (true);
		if (dropped) {

			JSONObject obj = JSONObject.Create ();
			obj.AddField ("clientId", connection.clientId);
			obj.AddField ("roomId", connection.roomId);

			socket.Emit ("rejoinRoom", obj, (SocketIOEvent s) => {
				Debug.Log ("RECONNECTED!!!");
				});

			dropped = false;
		}
	}

	void OnError (SocketIOEvent e) {
		/*#if UNITY_EDITOR
		Debug.LogWarning("[SocketIO] Error received: " + e.name + ", " + e.data);
		#endif*/
		if (e.name == "An exception has occurred while connecting.") {
			SendUpdateConnectionMessage (false);
		}
	}

	void OnClose (SocketIOEvent e) {
		SendUpdateConnectionMessage (false);
		if (dropped) {
			this.socket.Connect ();
		}
	}

	void SendUpdateConnectionMessage (bool connected) {
		if (onUpdateConnection != null)
			onUpdateConnection (connected);
	}

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
