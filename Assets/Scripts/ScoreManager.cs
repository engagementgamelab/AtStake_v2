using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using Models;

//// <summary>
/// Handles scoring for all players
/// This includes players' individual scores and the pot
/// </summary>
public class ScoreManager : GameInstanceBehaviour, IInventoryHolder {

	public delegate void OnUpdateScore (int score);

	Inventory inventory;
	public Inventory Inventory {
		get {
			if (inventory == null) {
				inventory = new Inventory (this);
				inventory.Add (new PotGroup (Settings.PotCoinCount));
			}
			return inventory;
		}
	}

	Settings settings;
	Settings Settings {
		get {
			if (settings == null) {
				settings = DataManager.GetSettings ();
			}
			return settings;
		}
	}

	public int PlayerScore {
		get { return Game.Manager.Player.CoinCount; }
		private set { 
			Game.Manager.Player.CoinCount = value; 
			SendUpdateMessage ();
		}
	}

	int DeciderScore {
		get { return Game.Manager.DeciderPlayer.CoinCount; }
		set { Game.Manager.DeciderPlayer.CoinCount = value; }
	}

	public int Pot {
		get { return Inventory["pot"].Count; }
	}

	public Dictionary<string, int> PlayerScores {
		get { 
			Dictionary<string, int> playerScores = new Dictionary<string, int> ();
			foreach (var player in Game.Manager.Players)
				playerScores.Add (player.Key, player.Value.CoinCount);
			return playerScores;
		}
	}

	public bool CanAffordExtraTime {
		get { return PlayerScore >= Settings.ExtraTimeCost; }
	}

	public OnUpdateScore onUpdateScore;

	public void Init () {
		PlayerScore = 0;
		Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
	}

	public void FillPot () { 
		Inventory["pot"].Set (Settings.PotCoinCount);
	}

	public void EmptyPot () { Inventory["pot"].Clear (); }

	public void AddRoundStartScores () {
		foreach (var player in Game.Manager.Players) {
			if (player.Key != Game.Manager.Decider)
				player.Value.CoinCount += Settings.PlayerStartCoinCount;	
		}
		DeciderScore += Settings.DeciderStartCoinCount;
		SendUpdateMessage ();
	}

	public void AddWinnings () {
		Game.Manager.Players[Game.Manager.Winner].CoinCount += Inventory["pot"].Count;
		EmptyPot ();
		SendUpdateMessage ();
	}

	public void ApplyPlayerReward (PlayerAgendaItem item) {
		Game.Manager.Players[item.Player].CoinCount += Settings.Rewards[item.Reward];
		SendUpdateMessage ();
	}

	void AcceptExtraTime (NetworkMessage msg) {
		Inventory["pot"].Add (Settings.ExtraTimeCost);
		Game.Manager.Players[msg.str1].CoinCount -= Settings.ExtraTimeCost;
		SendUpdateMessage ();
	}

	void SendUpdateMessage () {
		if (onUpdateScore != null)
			onUpdateScore (PlayerScore);
	}
}
