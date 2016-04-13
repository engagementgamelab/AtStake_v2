using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class NetManager2 {

	SocketIOComponent socket;

	struct ConnectionInfo {
		public string name;
		public string clientId;
		public Response.Room room;
	}

	ConnectionInfo connection;

	public NetManager2 (SocketIOComponent socket) {
		this.socket = socket;
		this.socket.On("open", OnOpen);
		this.socket.On("error", OnError);
		this.socket.On("close", OnClose);
		this.socket.Connect ();
	}

	public void StartAsHost (string name, Action<ResponseType> response) {
		connection.name = name;
		Register (name, () => {
			CreateRoom (response);
		});
	}

	public void RequestRoomList (Action<Dictionary<string, string>> response) {
		socket.Emit<Response.RoomList> ("requestRoomList", (Response.RoomList res) => {
			response(res.ToDictionary ());
		});
	}

	public void StartAsClient (string name, string roomId, Action<ResponseType> response) {
		connection.name = name;
		Register (name, () => {
			JoinRoom (roomId, response);
		});
	}

	public void SendMessage (string key, string str1, string str2, int val) {

		JSONObject obj = JSONObject.Create ();

		obj.AddField ("key", key);
		if (str1 != "")
			obj.AddField ("str1", str1);
		if (str2 != "")
			obj.AddField ("str2", str2);
		if (val != -1)
			obj.AddField ("val", val);

		/*socket.Emit ("send", obj, (JSONObject json) => {

		});*/
	}

	void Register (string name, Action callback) {

		// The client is only ever registered once. They remain in the database until the app is closed.
		if (!string.IsNullOrEmpty (connection.clientId))
			callback ();

		socket.Emit<Response.Registration> ("register", name, (Response.Registration res) => {
			connection.clientId = res.id;
			callback ();
		});
	}

	void CreateRoom (Action<ResponseType> callback) {
		
		#if UNITY_EDITOR
		if (string.IsNullOrEmpty (connection.clientId))
			throw new System.Exception ("Client must register before creating a room.");
		#endif
		
		socket.Emit<Response.CreateRoom> ("createRoom", connection.clientId, (Response.CreateRoom res) => {
			if (res.nameTaken) {
				callback (ResponseType.NameTaken);
			} else {
				connection.room = res.room;
				callback (ResponseType.Success);
			}
		});
	}

	void JoinRoom (string roomId, Action<ResponseType> callback) {

		JSONObject obj = JSONObject.Create ();
		obj.AddField ("clientId", connection.clientId);
		obj.AddField ("roomId", roomId);

		socket.Emit<Response.JoinRoom> ("joinRoom", obj, (Response.JoinRoom res) => {
			if (res.nameTaken) {
				callback (ResponseType.NameTaken);
			} else {
				connection.room = res.room;
				callback (ResponseType.Success);
			}
		});
	}

	void OnOpen (SocketIOEvent e) {
		Debug.Log ("[SocketIO] Open received: " + e.name + ", " + e.data);
	}

	void OnError (SocketIOEvent e) {
		Debug.Log("[SocketIO] Error received: " + e.name + ", " + e.data);
	}

	void OnClose (SocketIOEvent e) {
		Debug.Log ("[SocketIO] Close received: " + e.name + ", " + e.data);
	}

	class Response {

		public class Registration {
			public string id { get; set; }
		}

		public class CreateRoom {
			public Room room { get; set; }
			public bool nameTaken { get; set; }
		}

		public class JoinRoom {
			public Room room { get; set; }
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

		public class RoomListing {
			public string id { get; set; }
			public string host { get; set; }
		}

		public class Room {
			public string _id { get; set; }
		}
	}
}
