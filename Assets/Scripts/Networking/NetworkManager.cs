using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : GameInstanceComponent {

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

	[DebuggableMethod]
	public void HostGame () {
		Clients.Clear ();
		Hosting = true;
	}

	[DebuggableMethod]
	public void JoinGame (string gameName) {

		// For testing locally
		List<GameInstance> gi = ObjectPool.GetActiveInstances<GameInstance> ();
		foreach (GameInstance g in gi) {
			if (g.Name == gameName) {
				Host = g;
				Host.Network.Clients.Add (Game.Name, Game);
				return;
			}
		}
	}

	[DebuggableMethod]
	public void UpdateHosts () {
		hosts.Clear ();
		List<GameInstance> gi = ObjectPool.GetActiveInstances<GameInstance> ();
		foreach (GameInstance g in gi) {
			if (g.Network.Hosting && g != Game)
				hosts.Add (g.Name, g);
		}
	}

	[DebuggableMethod]
	public void Disconnect () {
		if (Hosting) {
			// TODO: disconnect all players currently in game
			Hosting = false;
		}
	}
}
