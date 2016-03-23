using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerManager2 : GameInstanceBehaviour {

	public delegate void OnLogMessage (string msg);

	public bool Hosting {
		get { return Host == Game.Name; }
	}

	public bool Connected {
		get { return Hosting || connected; }
		private set { connected = value; }
	}

	public string Host { get; private set; }

	public List<string> Clients {
		get { return clients; }
	}

	public NetworkMasterServer2 server;
	public NetworkMasterClient2 client;
	public OnLogMessage onLogMessage;

	Dictionary<string, string> hosts;
	List<string> clients = new List<string> ();
	System.Action<string> clientRegisterResponse;
	bool connected = false;

	void OnEnable () {
		server.onServerMessage += SendLogMessage;
		client.onClientMessage += SendLogMessage;
		MasterServerDiscovery.onLogMessage += SendLogMessage;
		client.callbacks.AddListener ("disconnected", OnDisconnect);
		client.onRegisteredClient += OnRegisteredClient;
		client.onUnregisteredClient += OnUnregisteredClient;
	}

	public void HostGame () {

		// Set this player as the host
		Host = Game.Name;

		// Start the server
		server.Initialize ();

		// Start the client and connect to the server as the host
		// Use the discovery service to broadcast this game
		client.StartAsHost (Host, () => {
			MasterServerDiscovery.StartBroadcasting (Host);
		});
	}

	public void RequestHostList (System.Action<List<string>> callback) {
		MasterServerDiscovery.StartListening (this, (Dictionary<string, string> hosts) => {
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
			MasterServerDiscovery.StopListening (this);
		});
	}

	// Intentional disconnect (player chose to terminate their connection)
	// This will stop the discovery service
	// If a connection has been established, the OnDisconnect event will also fire
	public void Disconnect () {
		if (Hosting) {
			MasterServerDiscovery.StopBroadcasting ();
			client.UnregisterHost (Host, () => {
				Co.WaitForFixedUpdate (() => {
					server.Reset ();
				});
			});
		} else {
			MasterServerDiscovery.StopListening (this);
			client.UnregisterClient ();
		}
	}

	void UpdatePlayers () {
		string players = "";
		foreach (string player in Clients) {
			players += player + "|";
		}
		players += Host;
		Debug.Log (players);
		// Dispatcher.ScheduleMessage ("UpdatePlayers", players);
	}

	// -- Messaging

	public void SendMessageToHost (MasterMsgTypes.GenericMessage msg) {

	}

	public void ReceiveMessageFromClient (MasterMsgTypes.GenericMessage msg) {

	}

	public void SendMessageToClients (MasterMsgTypes.GenericMessage msg) {

	}

	public void ReceiveMessageFromHost (MasterMsgTypes.GenericMessage msg) {

	}

	// -- Events

	// Intentional & unintentional disconnect
	void OnDisconnect () {
		if (Hosting) {
			MasterServerDiscovery.StopBroadcasting ();
		} else {
			MasterServerDiscovery.StopListening (this);
		}
		Host = "";
		Connected = false;
	}

	void OnRegisteredClient (int resultCode, string clientName) {
		
		bool thisClient = clientName == Game.Name;
		
		string keyword;
		switch (resultCode) {
			case -2: keyword = "room_full"; break;
			case -1: keyword = "name_taken"; break;
			default: 
				keyword = "registered"; 
				if (Hosting) {
					Clients.Add (clientName);
					UpdatePlayers ();
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
			Clients.Remove (clientName);
			UpdatePlayers ();
		}
	}

	void SendClientRegisterResponse (string response) {
		if (clientRegisterResponse != null) {
			clientRegisterResponse (response);
			clientRegisterResponse = null;
		}
	}

	// -- Debugging

	void SendLogMessage (string msg) {
		if (onLogMessage != null)
			onLogMessage (msg);
	}
}
