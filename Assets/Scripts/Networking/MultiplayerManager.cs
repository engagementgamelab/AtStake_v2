using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class MultiplayerManager : GameInstanceBehaviour {

	public delegate void OnLogMessage (string msg);
	public delegate void OnDisconnected ();

	/// <summary>
	/// Returns true if this is the game's host
	/// </summary>
	public bool Hosting { get; private set; }

	/// <summary>
	/// Returns true if connected (always true if hosting)
	/// </summary>
	public bool Connected {
		get { return Hosting || connected; }
		private set { connected = value; }
	}

	/// <summary>
	/// Gets/sets name of the game's host
	/// </summary>
	public string Host { get; private set; }

	/// <summary>
	/// If hosting, gets a list of client names connected to this host
	/// </summary>
	public List<string> Clients {
		get { return clients; }
	}

	/// <summary>
	/// Gets a list of host names populated from the master server
	/// </summary>
	public List<string> Hosts {
		get { return new List<string> (hosts.Keys); }
	}

	/// <summary>
	/// Returns true if the host was disconnected with an error. This flag is used to display an error message to the user.
	/// </summary>
	public bool DisconnectedWithError { get; private set; }

	public NetworkMasterServer server;
	public NetworkMasterClient client;
	public OnLogMessage onLogMessage;
	public OnDisconnected onDisconnected;

	Dictionary<string, string> hosts;
	List<string> clients = new List<string> ();
	bool connected = false;
	AvatarsManager avatars;

	public NetManager net;

	void OnEnable () {
		net = new NetManager (gameObject.AddComponent<SocketIOComponent> ());
		net.messageReceived += ReceiveMessageFromClient;
	}

	public void HostGame (Action<ResponseType> response) {

		// Set this player as the host
		Host = Game.Name;
		Hosting = true;
		DisconnectedWithError = false;

		// Initialize avatar manager and set the host's avatar
		avatars = new AvatarsManager ();
		avatars.AddPlayer (Host);
		Game.Manager.AddHost (avatars[Host]);

		net.StartAsHost (Game.Name, (ResponseType res) => {

			if (res == ResponseType.Success)
				net.clientsUpdated += OnUpdateClients;

			response (res);
		});
	}

	public void RequestHostList (Action<List<string>> callback) {

		net.RequestRoomList ((Dictionary<string, string> hosts) => {
			this.hosts = hosts;
			callback (new List<string> (hosts.Keys));
		});
	}

	public void JoinGame (string hostName, Action<ResponseType> response) {

		// Set the host
		Host = hostName;
		net.StartAsClient (Game.Name, hosts[Host], response);
	}

	public void GameStarted () {
		if (Hosting)
			net.CloseRoom ();
	}

	// Intentional disconnect (player chose to terminate their connection)
	// This will stop the discovery service
	// If a connection has been established, the OnDisconnect event will also fire
	public void Disconnect () {
		net.Stop ();
		OnDisconnect ();
	}

	// -- Client handling

	// Only the host uses these methods
	// Adds/removes clients to the list of clients as they join and leave the room
	// Enables/disables the discovery broadcaster based on whether or not there are available slots in the room
	// Sends a message whenever the client list is updated so that all players know who's in the game

	void AddClient (string clientName) {
		Clients.Add (clientName);
		avatars.AddPlayer (clientName);
		// UpdateBroadcast ();
		UpdatePlayers ();
	}

	void RemoveClient (string clientName) {
		Clients.Remove (clientName);
		avatars.RemovePlayer (clientName);
		// UpdateBroadcast ();
		UpdatePlayers ();
	}

	void UpdatePlayers () {
		Game.Dispatcher.ScheduleMessage ("UpdatePlayers", avatars.GetPlayers ());
	}

	// MasterServer
	/*void UpdateBroadcast () {
		int maxPlayerCount = DataManager.GetSettings ().PlayerCountRange[1];
		if (Clients.Count+1 < maxPlayerCount) {
			DiscoveryService.StartBroadcasting (Host, client.IpAddress, OnBroadcastError);
		} else {
			DiscoveryService.StopBroadcasting ();
		}
	}*/

	// -- Messaging

	public void ReceiveMessageFromClient (MasterMsgTypes.GenericMessage msg) {
		Game.Dispatcher.ReceiveMessage (msg);
	}

	public void SendMessageToClients (MasterMsgTypes.GenericMessage msg) {
		net.SendMessage (msg);
	}

	// -- Events

	// Intentional & unintentional disconnect
	void OnDisconnect () {
		if (Hosting) {
			Clients.Clear ();
			Hosting = false;
		}
		Host = "";
		Connected = false;
		if (onDisconnected != null)
			onDisconnected ();
	}

	void OnUpdateClients (string[] regClients) {

		// Add new clients
		foreach (string cl in regClients) {
			if (!Clients.Contains (cl))
				AddClient (cl);
		}

		// Remove old clients
		List<string> tempClients = new List<string> (Clients);
		foreach (string cl in tempClients) {
			if (Array.FindIndex (regClients, (string c) => { return c.Equals (cl); }) == -1)
				RemoveClient (cl);
		}
	}
}
