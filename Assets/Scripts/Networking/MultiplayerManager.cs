using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public enum ConnectionStatus {
	Searching,
	Success,
	Fail
}

public class MultiplayerManager : GameInstanceBehaviour {

	public delegate void OnLogMessage (string msg);
	public delegate void OnDisconnected ();
	public delegate void OnUpdateConnectionStatus (ConnectionStatus status);
	public delegate void OnUpdateDroppedClients (bool hasDroppedClients);

	/// <summary>
	/// Returns true if this is the game's host
	/// </summary>
	public bool Hosting { get; private set; }

	/// <summary>
	/// Returns true if connected to a game (always true if hosting)
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

	/// <summary>
	/// Returns true if the device is able to connect to the server.
	/// </summary>
	public ConnectionStatus ConnectionStatus {
		get { return connectionStatus; }
		private set { connectionStatus = value; }
	}

	public OnDisconnected onDisconnected;
	public OnUpdateConnectionStatus onUpdateConnectionStatus;
	public OnUpdateDroppedClients onUpdateDroppedClients;

	ConnectionStatus connectionStatus = ConnectionStatus.Searching;
	Dictionary<string, string> hosts;
	List<string> clients = new List<string> ();
	bool connected = false;
	AvatarsManager avatars;
	NetManager net;

	#if UNITY_EDITOR && SINGLE_SCREEN
	public string RoomId {
		get { return net.RoomId; }
	}
	#endif

	void Awake () {
		net = new NetManager (gameObject.AddComponent<SocketIOComponent> ());
		net.messageReceived += ReceiveMessageFromClient;
		net.onUpdateConnection += OnUpdateConnection;
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

			if (res == ResponseType.Success) {
				net.clientsUpdated = OnUpdateClients;
				net.onDisconnected = OnDisconnect;
				net.onUpdateDroppedClients = OnUpdateDropped;
			}

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
		JoinGame (hostName, hosts[hostName], response);
	}

	public void JoinGame (string hostName, string roomId, Action<ResponseType> response) {

		#if UNITY_EDITOR
		if (Connected) {
			Debug.LogWarning (Game.Name + " is attempting to join the '" + hostName + "' game but it is already connected to the '" + Host + "' game. This is not allowed: be sure to disconnect before joining a new room.");
			return;
		}
		#endif

		// Set the host
		Host = hostName;
		net.StartAsClient (Game.Name, roomId, (ResponseType res) => {

			if (res == ResponseType.Success) {
				Connected = true;
				net.onDisconnected = OnDisconnect;
				net.onUpdateDroppedClients = OnUpdateDropped;
			}

			response(res);	
		});
	}

	public void GameStarted () {
		if (Hosting)
			net.CloseRoom ();
	}

	// Intentional disconnect (player chose to terminate their connection)
	// The OnDisconnect event will also fire
	public void Disconnect () {
		net.Stop ();
	}

	void OnApplicationQuit () {
		Disconnect ();
	}

	/*void OnApplicationFocus (bool focused) {
		if (focused) {
			// reconnect dropped devices
		}
	}

	void OnApplicationPause(bool paused) {
	    if(paused) {
	       // Game is paused, remember the time
	    } else {
	       // Game is unpaused, calculate the time passed since the game was paused and use this time to calculate build times of your buildings or how much money the player has gained in the meantime.
	    }
	}*/

	// For testing only - simulate dropped devices
	public void Drop () {
		net.Drop ();
	}

	public void Reconnect () {
		net.Reconnect ();
	}

	// -- Client handling

	// Only the host uses these methods
	// Adds/removes clients to the list of clients as they join and leave the room
	// Enables/disables the discovery broadcaster based on whether or not there are available slots in the room
	// Sends a message whenever the client list is updated so that all players know who's in the game

	void AddClient (string clientName) {
		Clients.Add (clientName);
		avatars.AddPlayer (clientName);
		UpdatePlayers ();
	}

	void RemoveClient (string clientName) {
		Clients.Remove (clientName);
		avatars.RemovePlayer (clientName);
		UpdatePlayers ();
	}

	void UpdatePlayers () {
		Game.Dispatcher.ScheduleMessage ("UpdatePlayers", avatars.GetPlayers ());
	}

	// -- Messaging

	public void ReceiveMessageFromClient (NetMessage msg) {
		Game.Dispatcher.ReceiveMessage (msg);
	}

	public void SendMessageToClients (NetMessage msg) {
		net.SendMessage (msg);
	}

	// -- Events

	void OnUpdateConnection (bool connected) {
		ConnectionStatus = connected ? ConnectionStatus.Success : ConnectionStatus.Fail;
		if (onUpdateConnectionStatus != null)
			onUpdateConnectionStatus (ConnectionStatus);
	}

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

	// Clients list updated (only the host should subscribe to this event because the host will broadcast it to connected clients)
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

	void OnUpdateDropped (bool hasDroppedClients) {
		if (onUpdateDroppedClients != null)
			onUpdateDroppedClients (hasDroppedClients);
	}
}
