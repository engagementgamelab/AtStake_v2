using UnityEngine;
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

	bool connected = false;
	const float timeout = 3f;

	ResponseData.Room room;
	ResponseData.RoomList roomList;

	public void StartAsHost (string name, Action<bool> connected) {

		room = null;
		string maxPlayerCount = DataManager.GetSettings ().PlayerCountRange[1].ToString ();

		Request<ResponseData.Room> ((ResponseData.Room res) => {
			room = res;
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

		room = null;

		Request<ResponseData.Room> ((ResponseData.Room res) => {
			room = res;
			result ("registered");
		}, (string err) => {
			result (err);
		}, "registerClient", roomId, name, IpAddress);
	}

	void Request<T> (Action<T> response, Action<string> error, params string[] path) where T : ResponseData.Base {

		Co.WWW (BuildAddress (path), timeout, (WWW www) => {

			T res = JsonReader.Deserialize<T> (www.text);
			if (res.error != null && res.error != "")
				error (res.error);
			else
				response (res);

		}, (string err) => {
			error (err);
		});
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

	public class Client : Base {
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