using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerManager : GameInstanceBehaviour {

	public delegate void OnLogMessage (string msg);
	public delegate void OnDisconnected ();

	public bool Hosting { get; private set; }

	public bool Connected {
		get { return Hosting || connected; }
		private set { connected = value; }
	}

	public string Host { get; private set; }

	public List<string> Clients {
		get { return clients; }
	}

	public List<string> Hosts {
		get { return new List<string> (hosts.Keys); }
	}

	public bool DisconnectedWithError { get; private set; }

	public NetworkMasterServer server;
	public NetworkMasterClient client;
	public OnLogMessage onLogMessage;
	public OnDisconnected onDisconnected;

	Dictionary<string, string> hosts;
	List<string> clients = new List<string> ();
	System.Action<string> clientRegisterResponse;
	bool connected = false;

	void OnEnable () {

		// debugging messages
		server.onServerMessage += SendLogMessage;
		client.onClientMessage += SendLogMessage;

		// events
		client.callbacks.AddListener ("disconnected", OnDisconnect);
		client.onRegisteredClient += OnRegisteredClient;
		client.onUnregisteredClient += OnUnregisteredClient;
		client.onReceiveMessageFromHost += ReceiveMessageFromHost;
		client.onReceiveMessageFromClient += ReceiveMessageFromClient;
	}

	public void HostGame () {

		// Set this player as the host
		Host = Game.Name;
		Hosting = true;
		DisconnectedWithError = false;

		// Start the server
		server.Initialize ();

		// Start the client and connect to the server as the host
		// Use the discovery service to broadcast this game
		client.StartAsHost (Host, () => {
			DiscoveryService.StartBroadcasting (Host, client.IpAddress, OnBroadcastError);
		});
	}

	public void RequestHostList (System.Action<List<string>> callback) {
		DiscoveryService.StartListening (this, (Dictionary<string, string> hosts) => {
			this.hosts = hosts;
			callback (new List<string> (hosts.Keys));
		});
	}

	public void JoinGame (string hostName, System.Action<string> response) {

		// Set the host
		Host = hostName;

		// Setup response callback
		clientRegisterResponse += response;

		// Start the client and request to join the host's game
		// Stop the discovery service from listening for games to join
		client.StartAsClient (Game.Name, hosts[Host], () => {
			DiscoveryService.StopListening (this);
		});
	}

	public void JoinGame (string ipAddress) {
		Debug.Log (ipAddress);
		client.StartAsClient (Game.Name, ipAddress, () => {
			Debug.Log ("joined");
		});
	}

	// Intentional disconnect (player chose to terminate their connection)
	// This will stop the discovery service
	// If a connection has been established, the OnDisconnect event will also fire
	public void Disconnect () {
		if (Hosting) {
			DiscoveryService.StopBroadcasting ();
			client.UnregisterHost (Host, () => {
				Co.WaitForFixedUpdate (() => {
					server.Reset ();
				});
			});
		} else {
			DiscoveryService.StopListening (this);
			client.UnregisterClient ();
			OnDisconnect ();
		}
	}

	// -- Client handling

	// Only the host uses these methods
	// Adds/removes clients to the list of clients as they join and leave the room
	// Enables/disables the discovery broadcaster based on whether or not there are available slots in the room
	// Sends a message whenever the client list is updated so that all players know who's in the game

	void AddClient (string clientName) {
		Clients.Add (clientName);
		UpdateBroadcast ();
		UpdatePlayers ();
	}

	void RemoveClient (string clientName) {
		Clients.Remove (clientName);
		UpdateBroadcast ();
		UpdatePlayers ();
	}

	void UpdatePlayers () {
		string players = "";
		foreach (string player in Clients) {
			players += player + "|";
		}
		players += Host;
		Game.Dispatcher.ScheduleMessage ("UpdatePlayers", players);
	}

	void UpdateBroadcast () {
		int maxPlayerCount = DataManager.GetSettings ().PlayerCountRange[1];
		if (Clients.Count+1 < maxPlayerCount) {
			DiscoveryService.StartBroadcasting (Host, client.IpAddress, OnBroadcastError);
		} else {
			DiscoveryService.StopBroadcasting ();
		}
	}

	// -- Messaging

	public void SendMessageToHost (MasterMsgTypes.GenericMessage msg) {

		// Client sends message to host so that host can relay it to all clients
		client.SendMessageToHost (msg);
	}

	public void ReceiveMessageFromClient (MasterMsgTypes.GenericMessage msg) {

		// Host receives message from client so that it can relay it to all other clients
		Game.Dispatcher.ReceiveMessageFromClient (msg);
	}

	public void SendMessageToClients (MasterMsgTypes.GenericMessage msg) {

		// Host sends message to all clients
		client.SendMessageToClients (msg);
	}

	public void ReceiveMessageFromHost (MasterMsgTypes.GenericMessage msg) {

		// Clients receive message from host
		if (!Hosting) {
			Game.Dispatcher.ReceiveMessageFromHost (msg);
		}
	}

	// -- Events

	// Intentional & unintentional disconnect
	void OnDisconnect () {
		if (Hosting) {
			DiscoveryService.StopBroadcasting ();
			Clients.Clear ();
			Hosting = false;
		} else {
			DiscoveryService.StopListening (this);
		}
		Host = "";
		Connected = false;
		if (onDisconnected != null)
			onDisconnected ();
	}

	void OnRegisteredClient (int resultCode, string clientName) {
		
		bool thisClient = clientName == Game.Name;
		
		string keyword;
		switch (resultCode) {
			case -2: keyword = "room_full"; break; // (technically this should never happen because the host will stop broadcasting when the room is maxed out)
			case -1: keyword = "name_taken"; break;
			default: 
				keyword = "registered"; 
				if (Hosting) {
					AddClient (clientName);
				} else if (thisClient) {
					Connected = true;
				}
				break;
		}

		if (thisClient) {
			SendClientRegisterResponse (keyword);
		}
	}

	void OnUnregisteredClient (string clientName) {
		if (Hosting) {
			RemoveClient (clientName);
		}
	}

	void SendClientRegisterResponse (string response) {
		if (clientRegisterResponse != null) {
			clientRegisterResponse (response);
			clientRegisterResponse = null;
		}
	}

	void OnBroadcastError () {
		DisconnectedWithError = true;
		Disconnect ();
	}

	// -- Debugging

	void SendLogMessage (string msg) {
		if (onLogMessage != null)
			onLogMessage (msg);
	}
}
