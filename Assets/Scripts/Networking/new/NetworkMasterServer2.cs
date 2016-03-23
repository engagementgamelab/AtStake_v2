﻿using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NetworkMasterServer2 : MonoBehaviour {

	public delegate void OnServerMessage (string msg);

	class Settings {
		public readonly int MasterServerPort = 31485;
		public readonly bool LogMessagesInConsole = false;
	}

	class Room : MasterMsgTypes.Room {

		public void SetHost (string gameName, string ipAddress, int port, int connectId) {
			name = gameName.RemoveEmptyChars ();
			hostIp = ipAddress;
			hostPort = port;
			connectionId = connectId;
			playerLimit = DataManager.GetSettings ().PlayerCountRange[1];
			players = new MasterMsgTypes.Player[0];
		}

		public int AddPlayer (string playerName, int connectId) {

			// Don't add the player if the room is full (adding 1 to include the host in the count)
			if (players.Length+1 >= playerLimit)
				return -2;

			// Don't add the player if someone else in the room has the same name
			List<MasterMsgTypes.Player> playersList = new List<MasterMsgTypes.Player> (players);
			if (name == playerName || playersList.Find (x => x.name == playerName) != null)
				return -1;

			// Add the player
			MasterMsgTypes.Player player = new MasterMsgTypes.Player ();
			player.name = playerName;
			player.connectionId = connectId;
			playersList.Add (player);
			players = playersList.ToArray ();
			return 0;
		}

		public string RemovePlayer (int connectId) {

			if (connectionId == connectId) {
				throw new System.Exception ("Player could not be removed from the room because it is the host");
			}

			List<MasterMsgTypes.Player> playersList = players.ToList<MasterMsgTypes.Player> ();
			int index = playersList.FindIndex (x => x.connectionId == connectId);
			string clientName = playersList[index].name;
			if (index >= 0)
				playersList.RemoveAt (index);
			players = playersList.ToArray ();

			return clientName;
		}

		public bool HasPlayer (int connectId) {
			return players.ToList<MasterMsgTypes.Player> ().FindIndex (x => x.connectionId == connectId) >= 0;
		}
	}

	Settings settings = new Settings ();
	Room room = new Room ();

	public OnServerMessage onServerMessage;

	public void Initialize () {
		
		if (NetworkServer.active) {
			Debug.LogError ("Already initialized");
			return;
		}

		NetworkServer.Listen (settings.MasterServerPort);

		// server msgs
		NetworkServer.RegisterHandler (MsgType.Connect, OnConnect);
		NetworkServer.RegisterHandler (MsgType.Disconnect, OnDisconnect);
		NetworkServer.RegisterHandler (MsgType.Error, OnError);

		// application msgs
		NetworkServer.RegisterHandler (MasterMsgTypes.RegisterHostId, OnRegisterHost);
		NetworkServer.RegisterHandler (MasterMsgTypes.UnregisterHostId, OnUnregisterHost);
		NetworkServer.RegisterHandler (MasterMsgTypes.RegisterClientId, OnRegisterClient);
		NetworkServer.RegisterHandler (MasterMsgTypes.GenericClientToHostId, OnClientToHost);
		NetworkServer.RegisterHandler (MasterMsgTypes.GenericHostToClientsId, OnHostToClients);

		Log ("Server initialized");
	}

	public void Reset () {
		NetworkServer.Shutdown();
	}

	// -- Generic messages

	void OnClientToHost (NetworkMessage netMsg) {
		NetworkServer.SendToClient (
			room.connectionId,
			MasterMsgTypes.GenericHostFromClientId, 
			netMsg.ReadMessage<MasterMsgTypes.GenericMessage> ());
	}

	void OnHostToClients (NetworkMessage netMsg) {
		NetworkServer.SendToAll (
			MasterMsgTypes.GenericClientsFromHostId, 
			netMsg.ReadMessage<MasterMsgTypes.GenericMessage> ());
	}

	// -- System Handlers

	void OnConnect (NetworkMessage netMsg) { Log ("Master received client"); }
	void OnError (NetworkMessage netMsg) { Log ("Server received error"); }

	void OnDisconnect (NetworkMessage netMsg) {
		
		var msg = new MasterMsgTypes.UnregisteredClientMessage ();
		int connectionId = netMsg.conn.connectionId;

		// Only remove the player if they're in the room and not the host
		if (connectionId != room.connectionId && room.HasPlayer (connectionId)) {
			msg.clientName = room.RemovePlayer (netMsg.conn.connectionId);
			NetworkServer.SendToAll (MasterMsgTypes.UnregisteredClientId, msg);
		}
	}

	// -- Application Handlers

	void OnRegisterHost (NetworkMessage netMsg) {
		
		// Set the room host
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisterHostMessage> ();
		room.SetHost (msg.gameName, netMsg.conn.address, msg.hostPort, netMsg.conn.connectionId);

		// Send the message
		netMsg.conn.Send (MasterMsgTypes.RegisteredHostId, new MasterMsgTypes.RegisteredHostMessage ());
	}

	void OnUnregisterHost (NetworkMessage netMsg) {
		var response = new MasterMsgTypes.UnregisteredHostMessage ();
		response.resultCode = (int)MasterMsgTypes.NetworkMasterServerEvent.UnregistrationSucceeded;
		netMsg.conn.Send (MasterMsgTypes.UnregisteredHostId, response);
	}

	void OnRegisterClient (NetworkMessage netMsg) {

		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisterClientMessage> ();
		int result = room.AddPlayer (msg.clientName, netMsg.conn.connectionId);

		if (result == 0) {
			var response = new MasterMsgTypes.RegisteredClientMessage ();
			response.resultCode = (int)MasterMsgTypes.NetworkMasterServerEvent.RegisteredClientFailed;
			response.clientName = msg.clientName;
			NetworkServer.SendToAll (MasterMsgTypes.RegisteredClientId, response);
		} else {
			var err = new MasterMsgTypes.RegisteredClientMessage ();
			err.resultCode = result;
			err.clientName = msg.clientName;
			NetworkServer.SendToAll (MasterMsgTypes.RegisteredClientId, err);
		}
	}

	void Log (string msg) {
		if (settings.LogMessagesInConsole)
			Debug.Log (msg);
		if (onServerMessage != null)
			onServerMessage (msg);
	}
}
