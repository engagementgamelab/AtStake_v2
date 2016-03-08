#undef DEBUG
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class Rooms
{
	public string name;
	public Dictionary<string, MasterMsgTypes.Room> rooms = new Dictionary<string, MasterMsgTypes.Room>();

	public bool AddHost(string gameName, string comment, string hostIp, int hostPort, int connectionId)
	{
		if (rooms.ContainsKey(gameName))
		{
			return false;
		}

		MasterMsgTypes.Room room = new MasterMsgTypes.Room();
		room.name = gameName;
		room.comment = comment;
		room.hostIp = hostIp;
		room.hostPort = hostPort;
		room.connectionId = connectionId;
		room.playerLimit = DataManager.GetSettings ().PlayerCountRange[1];
		// room.players = new string[0];
		room.players = new MasterMsgTypes.Player[0];
		rooms[gameName] = room;

		return true;
	}

	public int AddPlayer (string gameName, string playerName, int connectionId) {

		// The name comes in with a bunch of empty chars. As cool & expected as that is, they have to be removed.
		gameName = gameName.RemoveEmptyChars ();

		MasterMsgTypes.Room room = rooms[gameName];

		// Don't add the player if the room is full (adding 1 to include the host in the count)
		if (room.players.Length+1 >= room.playerLimit)
			return -2;

		// Don't add the player if someone else in the room has the same name
		List<MasterMsgTypes.Player> playersList = new List<MasterMsgTypes.Player> (room.players);
		if (gameName == playerName || playersList.Find (x => x.name == playerName) != null)
			return -1;

		// Add the player
		MasterMsgTypes.Player player = new MasterMsgTypes.Player ();
		player.name = playerName;
		player.connectionId = connectionId;
		playersList.Add (player);
		room.players = playersList.ToArray ();
		return 0;
	}

	public string RemovePlayer (MasterMsgTypes.Room room, int connectionId) {

		string clientName = "";

		if (connectionId == room.connectionId) {
			throw new System.Exception ("Player could not be removed from the room because it is the host");
		} else {

			// client disconnected
			List<MasterMsgTypes.Player> playersList = room.players.ToList<MasterMsgTypes.Player> ();
			int index = playersList.FindIndex (x => x.connectionId == connectionId);
			clientName = playersList[index].name;
			if (index >= 0)
				playersList.RemoveAt (index);
			room.players = playersList.ToArray ();
		}

		return clientName;
	}

	public MasterMsgTypes.Room[] GetRooms()
	{
		return rooms.Values.ToArray();
	}
}

public class NetworkMasterServer : MonoBehaviour
{
	public int MasterServerPort;
	static NetworkMasterServer singleton;

	// map of gameTypeNames to rooms of that type
	Dictionary<string, Rooms> gameTypeRooms = new Dictionary<string, Rooms>();

	Rooms GetDefaultTypeRooms () {
		foreach (var rooms in gameTypeRooms)
			return rooms.Value;
		throw new System.Exception ("No rooms have been created");
	}

	// The way I've set it up, there should only ever be one room per server
	MasterMsgTypes.Room GetDefaultRoom () {
		try {
			return GetDefaultTypeRooms ().GetRooms ()[0];
		} catch {
			throw new System.Exception ("No rooms have been created");
		}
	}

	void Awake()
	{
		if (singleton == null)
		{
			singleton = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void InitializeServer()
	{
		if (NetworkServer.active)
		{
			Debug.LogError("Already Initialized");
			return;
		}

		NetworkServer.Listen(MasterServerPort);

		// system msgs
		NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnect);
		NetworkServer.RegisterHandler(MsgType.Disconnect, OnServerDisconnect);
		NetworkServer.RegisterHandler(MsgType.Error, OnServerError);

		// application msgs
		NetworkServer.RegisterHandler(MasterMsgTypes.RegisterHostId, OnServerRegisterHost);
		NetworkServer.RegisterHandler(MasterMsgTypes.UnregisterHostId, OnServerUnregisterHost);
		NetworkServer.RegisterHandler(MasterMsgTypes.RequestListOfHostsId, OnServerListHosts);
		NetworkServer.RegisterHandler(MasterMsgTypes.RegisterClientId, OnServerRegisterClient);
		NetworkServer.RegisterHandler(MasterMsgTypes.GenericClientToHostId, OnServerClientToHost);
		NetworkServer.RegisterHandler(MasterMsgTypes.GenericHostToClientsId, OnServerHostToClients);
	}

	public void ResetServer()
	{
		NetworkServer.Shutdown();
	}

	Rooms EnsureRoomsForGameType(string gameTypeName)
	{
		if (gameTypeRooms.ContainsKey(gameTypeName))
		{
			return gameTypeRooms[gameTypeName];
		}

		Rooms newRooms = new Rooms();
		newRooms.name = gameTypeName;
		gameTypeRooms[gameTypeName] = newRooms;
		return newRooms;
	}

	// --------------- System Handlers -----------------

	void OnServerConnect(NetworkMessage netMsg)
	{
		Debug.Log("Master received client");
	}

	void OnServerDisconnect(NetworkMessage netMsg)
	{
		Debug.Log("Master lost client");

		// Host disconnected
		foreach (var rooms in gameTypeRooms.Values)
		{
			foreach (var room in rooms.rooms.Values)
			{
				if (room.connectionId == netMsg.conn.connectionId)
				{
					// tell other players?

					// remove room
					rooms.rooms.Remove(room.name);

					Debug.Log("Room ["+room.name+"] closed because host left");
					break;
				}
			}
		}

		if (gameTypeRooms.Count > 0) {

			// Client disconnected
			var msg = new MasterMsgTypes.UnregisteredClientMessage ();
			msg.clientName = GetDefaultTypeRooms ().RemovePlayer (GetDefaultRoom (), netMsg.conn.connectionId);
			NetworkServer.SendToAll (MasterMsgTypes.UnregisteredClientId, msg);
		}
	}

	void OnServerError(NetworkMessage netMsg)
	{
		Debug.Log("ServerError from Master");
	}

	// --------------- Application Handlers -----------------

	void OnServerRegisterHost(NetworkMessage netMsg)
	{
		// Debug.Log("OnServerRegisterHost");
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisterHostMessage>();
		var rooms = EnsureRoomsForGameType(msg.gameTypeName);

		int result = (int)MasterMsgTypes.NetworkMasterServerEvent.RegistrationSucceeded;
		if (!rooms.AddHost(msg.gameName, msg.comment, netMsg.conn.address, msg.hostPort, netMsg.conn.connectionId))
		{
			result = (int)MasterMsgTypes.NetworkMasterServerEvent.RegistrationFailedGameName;
		}

		var response = new MasterMsgTypes.RegisteredHostMessage();
		response.resultCode = result;
		netMsg.conn.Send(MasterMsgTypes.RegisteredHostId, response);
	}

	void OnServerUnregisterHost(NetworkMessage netMsg)
	{
		Debug.Log("OnServerUnregisterHost");
		var msg = netMsg.ReadMessage<MasterMsgTypes.UnregisterHostMessage>();

		// find the room
		var rooms = EnsureRoomsForGameType(msg.gameTypeName);
		if (!rooms.rooms.ContainsKey(msg.gameName))
		{
			//error
			Debug.Log("OnServerUnregisterHost game not found: " + msg.gameName);
			return;
		}

		var room = rooms.rooms[msg.gameName];
		if (room.connectionId != netMsg.conn.connectionId)
		{
			//error
			Debug.Log("OnServerUnregisterHost connection mismatch:" + room.connectionId);
			return;
		}
		rooms.rooms.Remove(msg.gameName);

		// tell other players?

		var response = new MasterMsgTypes.RegisteredHostMessage();
		response.resultCode = (int)MasterMsgTypes.NetworkMasterServerEvent.UnregistrationSucceeded;
		netMsg.conn.Send(MasterMsgTypes.UnregisteredHostId, response);
	}

	void OnServerListHosts(NetworkMessage netMsg)
	{
		Debug.Log("OnServerListHosts");
		var msg = netMsg.ReadMessage<MasterMsgTypes.RequestHostListMessage>();
		if (!gameTypeRooms.ContainsKey(msg.gameTypeName))
		{
			var err = new MasterMsgTypes.ListOfHostsMessage();
			err.resultCode = -1;
			netMsg.conn.Send(MasterMsgTypes.ListOfHostsId, err);
			return;
		}

		var rooms = gameTypeRooms[msg.gameTypeName];
		var response = new MasterMsgTypes.ListOfHostsMessage();
		response.resultCode = 0;
		response.hosts = rooms.GetRooms();
		netMsg.conn.Send(MasterMsgTypes.ListOfHostsId, response);
	}

	void OnServerRegisterClient(NetworkMessage netMsg)
	{
		Debug.Log ("Client registered");
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisterClientMessage>();
		if (!gameTypeRooms.ContainsKey(msg.gameTypeName))
		{
			throw new System.Exception ("No game with the name '" + msg.gameTypeName + "' exists");
		}

		var rooms = gameTypeRooms[msg.gameTypeName];
		int addPlayerResult = rooms.AddPlayer (msg.gameName, msg.clientName, netMsg.conn.connectionId);

		switch (addPlayerResult) {
			case 0:
				var response = new MasterMsgTypes.RegisteredClientMessage ();
				response.resultCode = (int)MasterMsgTypes.NetworkMasterServerEvent.RegisteredClientFailed;
				response.clientName = msg.clientName;
				NetworkServer.SendToAll (MasterMsgTypes.RegisteredClientId, response);
				break;
			default:
				var err = new MasterMsgTypes.RegisteredClientMessage ();
				err.resultCode = addPlayerResult;
				err.clientName = msg.clientName;
				NetworkServer.SendToAll (MasterMsgTypes.RegisteredClientId, err);
				break;
		}
	}

	void OnServerClientToHost (NetworkMessage netMsg) {
		NetworkServer.SendToClient (
			GetDefaultRoom ().connectionId,
			MasterMsgTypes.GenericHostFromClientId, 
			netMsg.ReadMessage<MasterMsgTypes.GenericMessage> ());
	}

	void OnServerHostToClients (NetworkMessage netMsg) {
		NetworkServer.SendToAll (
			MasterMsgTypes.GenericClientsFromHostId, 
			netMsg.ReadMessage<MasterMsgTypes.GenericMessage> ());
	}

	#if DEBUG
	void OnGUI()
	{
		if (NetworkServer.active)
		{
			GUI.Label(new Rect(400, 0, 200, 20), "Online port:" + MasterServerPort);
			if (GUI.Button(new Rect(400, 20, 200, 20), "Reset  Master Server"))
			{
				ResetServer();
			}
		}
		else
		{
			if (GUI.Button(new Rect(400, 20, 200, 20), "Init Master Server"))
			{
				InitializeServer();
			}
		}

		int y = 100;
		foreach (var rooms in gameTypeRooms.Values)
		{
			GUI.Label(new Rect(400, y, 200, 20), "GameType:" + rooms.name);
			y += 22;
			foreach (var room in rooms.rooms.Values)
			{
				GUI.Label(new Rect(420, y, 200, 20), "Game:" + room.name + " addr:" + room.hostIp + ":" + room.hostPort);
				y += 22;
			}
		}
	}
	#endif
}
