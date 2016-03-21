using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//// <summary>
/// Handles local multiplayer
/// Games consist of a host and multiple clients
/// </summary>
public class MultiplayerManager : GameInstanceBehaviour {

	public delegate void OnConnect ();
	public delegate void OnDisconnect ();
	public delegate void OnUpdateClients (List<string> clients);
	public delegate void OnRoomFull ();
	public delegate void OnNameTaken ();

	public string Host { get; private set; }
	List<string> hosts = new List<string> ();
	List<string> clients = new List<string> ();

	public List<string> Hosts {
		get { return hosts; }
	}

	public List<string> Clients {
		get { return clients; }
	}

	public bool Hosting {
		get { return Host == Game.Name; }
	}

	bool connected = false;
	public bool Connected {
		get { return Hosting || connected; }
		set { connected = value;}
	}

	public OnConnect onConnect;
	public OnDisconnect onDisconnect;
	public OnUpdateClients onUpdateClients;
	public OnRoomFull onRoomFull;
	public OnNameTaken onNameTaken;

	public NetworkingManager networking;

	NetworkingManager connectionManager;
	NetworkingManager ConnectionManager {
		get {
			connectionManager = networking;
			connectionManager.Init (Game.Name, this);
			return connectionManager;
		}
	}

	void OnEnable () {
		networking.gameObject.SetActive (true);
	}

	// Host
	public void HostGame () {
		Clients.Clear ();
		Host = Game.Name;
		ConnectionManager.Host ();
	}

	// Client
	public void JoinGame (string hostName) {
		Host = hostName;
		ConnectionManager.Join (hostName);
	}

	// Client
	public void RequestHostList (System.Action<List<string>> callback) {
		ConnectionManager.RequestHostList ((List<string> result) => {
			hosts = result;
			callback (result);
		});
	}

	// Host
	void ConnectClient (string clientName) {
		Clients.Add (clientName);
		if (onUpdateClients != null)
			onUpdateClients (Clients);
	}

	// Host & Client
	public void Disconnect () {
		ConnectionManager.Disconnect (Host);
		OnDisconnected ();
	}

	// Host & Client
	public void OnDisconnected () {
		Host = "";
		Connected = false;
		if (onDisconnect != null)
			onDisconnect ();
	}

	// Host & Client
	public void OnRegisteredClient (int resultCode, string clientName) {

		bool thisClient = clientName == Game.Name;

		switch (resultCode) {
			case -1:
				if (!Hosting && thisClient) {
					if (onNameTaken != null)
						onNameTaken ();
				}
				break;
			case -2:
				if (!Hosting && thisClient) {
					if (onRoomFull != null)
						onRoomFull ();
				}
				break;
			default:
				if (Hosting) {
					ConnectClient (clientName);
				} else if (thisClient) {
					Connected = true;
					if (onConnect != null) {
						onConnect ();
					}
				}
				break;
		}
	}

	// Host
	public void OnUnregisteredClient (string name) {
		if (Hosting) {
			Clients.Remove (name);
			if (onUpdateClients != null)
				onUpdateClients (Clients);
		}
	}

	public void SendMessageToHost (MasterMsgTypes.GenericMessage msg) {
		ConnectionManager.SendMessageToHost (msg);
	}

	public void ReceiveMessageFromClient (MasterMsgTypes.GenericMessage msg) {

		// Only the host receives this message
		Game.Dispatcher.ReceiveMessageFromClient (msg);
	}

	public void SendMessageToClients (MasterMsgTypes.GenericMessage msg) {
		ConnectionManager.SendMessageToClients (msg);
	}

	public void ReceiveMessageFromHost (MasterMsgTypes.GenericMessage msg) {

		// Pass this on to all clients except the host
		if (!Hosting) {
			Game.Dispatcher.ReceiveMessageFromHost (msg);
		}
	}
}
