using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	// Host
	public void HostGame () {
		Hosting = true;
	}

	// Client
	public void JoinGame (string gameName) {

		// For testing locally
		List<GameInstance> gi = ObjectPool.GetActiveInstances<GameInstance> ();
		foreach (GameInstance g in gi) {
			if (g.Name == gameName) {
				Host = g;
				Host.Multiplayer.ConnectClient (Game);
				return;
			}
		}
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
