using UnityEngine;
using System;
using System.Collections;
using SocketIO;

public class NetManager2 {

	SocketIOComponent socket;

	struct ConnectionInfo {
		public string clientId;
	}

	ConnectionInfo connection;

	public NetManager2 (SocketIOComponent socket) {
		this.socket = socket;
		this.socket.On("open", OnOpen);
		this.socket.On("error", OnError);
		this.socket.On("close", OnClose);
		this.socket.Connect ();
	}

	public void Register (string name) {
		socket.Emit<Response.Registration> ("register", name, (Response.Registration res) => {
			Debug.Log (res.id);
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
	}
}
