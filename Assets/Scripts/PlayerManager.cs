using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using Models;

public class PlayerManager : GameInstanceBehaviour, IInventoryHolder {

	public delegate void OnAddPeer (string peer);
	public delegate void OnRemovePeer (string peer);

	// All players (including this player)
	public Dictionary<string, Player> Players {
		get {
			Dictionary<string, Player> players = new Dictionary<string, Player> (peers);
			players.Add (Player.Name, Player);
			return players;
		}
	}

	// The players' names (including this one)
	public List<string> PlayerNames {
		get { return new List<string> (Players.Keys); }
	}

	// Every player except this one
	Dictionary<string, Player> peers = new Dictionary<string, Player> ();
	public Dictionary<string, Player> Peers {
		get { return peers; }
	}

	// This player
	Player player;
	public Player Player {
		get {
			if (player == null) {
				player = new Models.Player ();
			}
			return player;
		}
	}

	Inventory inventory;
	public Inventory Inventory {
		get {
			if (inventory == null) {
				inventory = new Inventory (this);
				inventory.Add (new CoinGroup ());
				inventory.Add (new PotGroup ());
			}
			return inventory;
		}
	}
	
	public OnAddPeer onAddPeer;
	public OnRemovePeer onRemovePeer;

	public void Init () {
		Game.Dispatcher.AddListener ("UpdatePlayers", OnUpdatePlayers);
		Inventory["coins"].Clear ();
		Inventory["pot"].Clear ();
		peers.Clear ();
	}

	public void OnUpdatePlayers (NetworkMessage msg) {

		List<string> players = new List<string> (msg.str1.Split ('|'));
		players.Remove (Player.Name);

		foreach (string newPeer in players) {
			if (!peers.ContainsKey (newPeer)) {
				peers.Add (newPeer, new Player { Name = newPeer });
				if (onAddPeer != null)
					onAddPeer (newPeer);
			}
		}

		Dictionary<string, Player> tempPeers = new Dictionary<string, Player> (peers);

		foreach (var peer in tempPeers) {
			string peerName = peer.Key;
			if (!players.Contains (peerName)) {
				peers.Remove (peerName);
				if (onRemovePeer != null)
					onRemovePeer (peerName);
			}
		}
	}

	public void AddPeer (string name) {
		peers.Add (name, new Player { Name = name });
	}

	public void RemovePeer (string name) {
		peers.Remove (name);
	}
}
