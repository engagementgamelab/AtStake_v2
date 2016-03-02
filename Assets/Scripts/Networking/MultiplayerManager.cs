using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//// <summary>
/// Handles local multiplayer
/// Games consist of a host and multiple clients
/// </summary>
public class MultiplayerManager : GameInstanceBehaviour {

	public delegate void OnDisconnect ();
	public delegate void OnUpdateClients (List<string> clients);

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

	public OnDisconnect onDisconnect;
	public OnUpdateClients onUpdateClients;

	public NetworkingManager networking;
	public BluetoothManager bluetooth;
	public LocalManager local;

	IConnectionManager connectionManager;
	IConnectionManager ConnectionManager {
		get {
			if (connectionManager == null) {
				#if SINGLE_SCREEN
				connectionManager = local;
				#else
				if (networking.Status == ConnectionStatus.Succeeded) {
					connectionManager = networking;
				} else {
					connectionManager = bluetooth;
				}
				#endif
			}
			connectionManager.Init (Game.Name);
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
	public List<string> UpdateHosts () {
		hosts = ConnectionManager.UpdateHosts ();
		return hosts;
	}

	// Host
	public void ConnectClient (string clientName) {
		ConnectionManager.ConnectClient (clientName);
		Clients.Add (clientName);
		if (onUpdateClients != null)
			onUpdateClients (Clients);
	}

	// Host
	public void DisconnectClient (string name) {
		Clients.Remove (name);
		if (onUpdateClients != null)
			onUpdateClients (Clients);
	}

	// Host & Client
	public void Disconnect () {
		ConnectionManager.Disconnect (Host);
		if (onDisconnect != null)
			onDisconnect ();
	}

	public void SendMessageToHost (string id, string str1, string str2, int val) {
		ConnectionManager.SendMessageToHost (id, str1, str2, val);
	}

	public void ReceiveMessageFromClient (string id, string str1, string str2, int val) {
		ConnectionManager.ReceiveMessageFromClient (id, str1, str2, val);	
	}

	public void SendMessageToClients (string id, string str1, string str2, int val) {
		ConnectionManager.SendMessageToClients (id, str1, str2, val);
	}

	public void ReceiveMessageFromHost (string id, string str1, string str2, int val) {
		ConnectionManager.ReceiveMessageFromHost (id, str1, str2, val);
	}
}
