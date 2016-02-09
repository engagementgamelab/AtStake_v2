using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using Models;

// TODO: rename to GameData
public class PlayerManager : GameInstanceComponent, IInventoryHolder {

	public delegate void OnAddPeer (string peer);
	public delegate void OnRemovePeer (string peer);

	Dictionary<string, Player> peers = new Dictionary<string, Player> ();
	public Dictionary<string, Player> Peers {
		get { return peers; }
	}

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
		Inventory["coins"].Clear ();
		Inventory["pot"].Clear ();
		peers.Clear ();
	}

	public void UpdatePeers (List<string> newPeers) {

		foreach (string newPeer in newPeers) {
			if (!peers.ContainsKey (newPeer)) {
				AddPeer (newPeer);
				if (onAddPeer != null)
					onAddPeer (newPeer);
			}
		}

		foreach (var peer in peers) {
			if (!newPeers.Contains (peer.Key)) {
				RemovePeer (peer.Key);
				if (onRemovePeer != null)
					onRemovePeer (peer.Key);
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
