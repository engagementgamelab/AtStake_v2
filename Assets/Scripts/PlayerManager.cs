using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using Models;

// Send all messages through PlayerManager
// Caches game state

public class PlayerManager : GameInstanceComponent, IInventoryHolder {

	Dictionary<string, Player> players = new Dictionary<string, Player> ();

	Inventory inventory;
	public Inventory Inventory {
		get {
			if (inventory == null) {
				inventory = new Inventory (this);
				inventory.Add (new CoinGroup ()); // The pot
			}
			return inventory;
		}
	}

	public void AddPlayer (Player player) {
		players.Add (player.Name, player);
	}

	public void RemovePlayer (string name) {
		try {
			players.Remove (name);
		} catch {
			throw new System.Exception ("No player named '" + name + "' has been added to the game");
		}
	}
}
