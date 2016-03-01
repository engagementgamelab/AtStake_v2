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

	bool hosting = false;
	public bool Hosting { 
		get { return hosting; }
		private set { hosting = value; }
	}

	public GameInstance Host { get; private set; }

	Dictionary<string, GameInstance> hosts = new Dictionary<string, GameInstance> ();
	public List<string> Hosts {
		get { return new List<string> (hosts.Keys); }
	}

	Dictionary<string, GameInstance> clients = new Dictionary<string, GameInstance> ();
	public Dictionary<string, GameInstance> Clients {
		get { return clients; }
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
			return connectionManager;
		}
	}

	void OnEnable () {
		networking.gameObject.SetActive (true);
	}

	// Host
	public void HostGame () {
		Hosting = true;
		ConnectionManager.Host (Game.Name);
	}

	// Client
	public void JoinGame (string hostName) {

		// For testing locally
		/*List<GameInstance> gi = ObjectPool.GetActiveInstances<GameInstance> ();
		foreach (GameInstance g in gi) {
			if (g.Name == hostName) {
				Host = g;
				Host.Multiplayer.ConnectClient (Game);
				return;
			}
		}*/

		ConnectionManager.Join (hostName, Game.Name);
	}

	// Client
	public List<string> UpdateHosts () {
		hosts.Clear ();
		List<GameInstance> gi = ObjectPool.GetActiveInstances<GameInstance> ();
		foreach (GameInstance g in gi) {
			if (g.Multiplayer.Hosting && g != Game)
				hosts.Add (g.Name, g);
		}
		return new List<string> (hosts.Keys);
	}

	// Host
	public void ConnectClient (GameInstance game) {
		Clients.Add (game.Name, game);
		if (onUpdateClients != null)
			onUpdateClients (new List<string> (Clients.Keys));
	}

	// Host
	public void DisconnectClient (string name) {
		Clients.Remove (name);
		if (onUpdateClients != null)
			onUpdateClients (new List<string> (Clients.Keys));
	}

	// Host & Client
	public void Disconnect () {
		if (Hosting) {
			Hosting = false;
			if (onDisconnect != null)
				onDisconnect ();
			// TODO: disconnect all players currently in game
			foreach (var client in new Dictionary<string, GameInstance> (Clients)) {
				MultiplayerManager nm = Clients[client.Key].Multiplayer;
				nm.Host = null;
				nm.Disconnect ();
			}
		} else {
			if (Host != null)
				Host.Multiplayer.DisconnectClient (Game.Name);
			if (onDisconnect != null)
				onDisconnect ();
		}
	}
}
