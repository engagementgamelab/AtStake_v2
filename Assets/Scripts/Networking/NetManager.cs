using UnityEngine;
using UnityEngine.Experimental.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class NetManager {

	string Address {
		get { return DataManager.MasterServerAddress; }
	}

	string IpAddress {
		#if SINGLE_SCREEN
		get { return "127.0.0.1"; }
		#else
		get { return Network.player.ipAddress; }
		#endif
	}

	const float timeout = 3f;
	ResponseData.RoomList roomList;

	struct ConnectionInfo {
		
		public ResponseData.Room room;
		public string name;

		public bool Connected {
			get { return room == null; }
		}

		public bool Hosting {
			get { return room.host.name == name; }
		}

		public string ClientId {
			get { 
				string n = name;
				ResponseData.Client myClient = Array.Find (room.clients, x => x.name == n);
				if (myClient == null)
					myClient = room.host;
				return myClient._id; 
			}
		}

		public string RoomId {
			get { return room._id; }
		}

		public void Connect (ResponseData.Room connectRoom, string connectName) {
			room = connectRoom;
			name = connectName;
		}

		public void Reset () {
			room = null;
			name = "";
		}
	}

	ConnectionInfo connection;

	// -- Public API

	public void StartAsHost (string name, Action<bool> connected) {

		string maxPlayerCount = DataManager.GetSettings ().PlayerCountRange[1].ToString ();

		Request<ResponseData.Room> ((ResponseData.Room res) => {
			connection.Connect (res, name);
			connected (true);
		}, (string err) => {
			connected (false);
		}, "registerHost", name, IpAddress, maxPlayerCount);
	}

	public void RequestRoomList (Action<Dictionary<string, string>> rooms) {

		roomList = null;

		Request<ResponseData.RoomList> ((ResponseData.RoomList res) => {
			rooms (res.ToDictionary ());
		}, (string err) => {
			rooms (new Dictionary<string, string> ());
		}, "roomList");
	}

	public void StartAsClient (string roomId, string name, Action<string> result) {

		Request<ResponseData.Room> ((ResponseData.Room res) => {
			connection.Connect (res, name);
			result ("registered");
		}, (string err) => {
			result (err);
		}, "registerClient", roomId, name, IpAddress);
	}

	public void ListenForClients (Action<string[]> clients) {

		#if UNITY_EDITOR
		if (connection.Connected)
			throw new Exception ("The method ListenForClients requires that the device be connected to a room as the host");
		#endif

		RepeatRequest<ResponseData.Room> ((ResponseData.Room res) => {
			 clients (Array.ConvertAll (res.clients, x => x.name));
		}, (string err) => {
			Debug.LogError (err);
		}, "getRoom", connection.RoomId);
	}

	public void Stop () {

		#if UNITY_EDITOR
		if (connection.Connected) {
			Debug.LogWarning ("Client cannot be unregistered because it is not connected to the server");
			return;
		}
		#endif

		string[] path = connection.Hosting
			? new string[] { "unregisterHost", connection.RoomId }
			: new string[] { "unregisterClient", connection.RoomId, connection.ClientId };

		connection.Reset ();

		Request ((ResponseData.Base res) => {
			Debug.Log ("successful disconnect!");
		}, (string err) => {
			Debug.LogError (err);
		}, path);
	}

	// TODO: clean this up
	WWWForm form;

	public void SendMessage (MasterMsgTypes.GenericMessage msg) {

		string str1 = msg.str1;
		if (str1 == "")
			str1 = "_";
		string str2 = msg.str2;
		if (str2 == "")
			str2 = "_";
		string val = msg.val.ToString ();

		Request ((ResponseData.Base res) => {
			Debug.Log ("sent message " + msg.id);
		}, (string err) => {
			Debug.LogError (err);
		}, "sendMessage", connection.RoomId, connection.ClientId, msg.id, str1, str2, val);

		/*form = new WWWForm ();
		form.AddField ("key", msg.id);
		form.AddField ("str1", msg.str1);
		form.AddField ("str2", msg.str2);
		form.AddField ("val", msg.val);
		if (msg.bytes != null)
			form.AddBinaryData ("bytes", msg.bytes);

		Co.StartCoroutine (CoSendForm);*/
	}

	/*IEnumerator CoSendForm () {
		
		WWW www = new WWW (BuildAddress ("sendMessage", connection.RoomId, connection.ClientId), form);
		yield return www;
		if (!string.IsNullOrEmpty (www.error))
			Debug.LogError (www.error);
		else
			Debug.Log ("sent form :)");
	}*/

	public void ReceiveMessage (Action<MasterMsgTypes.GenericMessage> onReceiveMessage) {
		RepeatRequest ((ResponseData.Message res) => {
			if (res.result != "no_messages" && res.key != null) {
				onReceiveMessage (MasterMsgTypes.GenericMessage.Create (
					res.key,
					res.str1 == "_" ? "" : res.str1,
					res.str2 == "_" ? "" : res.str2,
					res.val)
				);
			}
		}, (string err) => {
			Debug.LogError (err);
		}, "receiveMessage", connection.RoomId, connection.ClientId);
	}

	// -- Private methods

	void Request (Action<ResponseData.Base> response, Action<string> error, params string[] path) {
		Request<ResponseData.Base> (response, error, path);
	}

	void Request<T> (Action<T> response, Action<string> error, params string[] path) where T : ResponseData.Base {

		Co.WWW (BuildAddress (path), timeout, (WWW www) => {

			// T res = JsonReader.Deserialize<T> (www.downloadHandler.text);
			T res = JsonReader.Deserialize<T> (www.text);
			if (res.error != null && res.error != "")
				error (res.error);
			else
				response (res);

		}, (string err) => {
			error (err);
		});
	}

	void RepeatRequest<T> (Action<T> response, Action<string> onError, params string[] path) where T : ResponseData.Base {

		// Stop the loop if the client is no longer connected to the room
		if (connection.Connected)
			return;

		Request<T> ((T t) => {
			response (t);
			RepeatRequest (response, onError, path);
		}, onError, path);
	}

	void EstablishConnection (Action<bool> callback) {
		Co.WWW (BuildAddress ("ping"), 3f, (WWW www) => {
			callback (true);
		}, (string error) => {
			callback (false);
			EstablishConnection (callback);
		});
	}

	string BuildAddress (params string[] path) {
		return Address + "/" + string.Join ("/", path);
	}
}

public class ResponseData {

	public class Base {
		public string error { get; set; }
		public string result { get; set; }
	}

	public class Room : Base {
		public string _id { get; set; }
		public Client host { get; set; }
		public Client[] clients { get; set; }
		public Message[] messages { get; set; }
	}

	public class RoomListing : Base {
		public string roomId { get; set; }
		public string host { get; set; }
	}

	public class RoomList : Base {

		public RoomListing[] rooms { get; set; }

		public Dictionary<string, string> ToDictionary () {
			Dictionary<string, string> r = new Dictionary<string, string> ();
			foreach (RoomListing room in rooms)
				r.Add (room.host, room.roomId);
			return r;
		}
	}

	public class ClientList : Base {
		public Client[] clients { get; set; }
	}

	public class Client : Base {
		public string _id { get; set; }
		public string name { get; set; }
		public string address { get; set; }
	}

	public class Message : Base {
		public string key { get; set; }
		public string str1 { get; set; }
		public string str2 { get; set; }
		public int val { get; set; }
	}
}

/*

app.get('/ping', function(req, res) { res.status(200).json({ result: "sucess" })});

// -- Rooms
app.get('/registerHost/:name/:address/:maxClientCount', rooms.registerHost);
app.get('/unregisterHost/:roomId', rooms.unregisterHost);
app.get('/registerClient/:roomId/:name/:address', rooms.registerClient);
app.get('/unregisterClient/:roomId/:clientId', rooms.unregisterClient);
app.get('/roomList', rooms.requestRoomList);
app.get('/closeRoom/:roomId', rooms.close);
app.get('/closeDisconnectedRooms', rooms.closeDisconnectedRooms);

// -- Messaging
app.get('/sendMessage/:roomId/:clientId/:key/:str1/:str2/:val', messages.send);
app.get('/receiveMessage/:roomId/:clientId', messages.receive);

// -- Debugging
app.get('/reset', rooms.reset);
app.get('/printRooms', rooms.printRooms);

*/