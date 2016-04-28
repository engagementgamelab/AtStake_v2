using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Models;

//// <summary>
/// Interfaces with GameController to handle scoring for all players
/// This includes players' individual scores and the pot
/// </summary>
public class ScoreManager : GameInstanceBehaviour {

	public delegate void OnUpdateScore (int score);
	public delegate void OnUpdatePot (int pot);

	/// <summary>
	/// Gets/sets this player's score. Sends a notification that the score has been updated.
	/// </summary>
	public int PlayerScore {
		get { return Game.Controller.CoinCount; }
		set { 
			Game.Controller.CoinCount = value; 
			SendUpdateMessage ();
		}
	}
	
	/// <summary>
	/// Gets/sets the pot value. Sends a notification that the pot has been updated.
	/// </summary>
	public int Pot {
		get { return Game.Controller.Pot; }
		set { 
			Game.Controller.Pot = value; 
			SendUpdatePotMessage ();
		}
	}

	/// <summary>
	/// Gets the scores for each player. The key is the player's name.
	/// </summary>
	public Dictionary<string, int> PlayerScores {
		get {
			Dictionary<string, int> playerScores = new Dictionary<string, int> ();
			foreach (Player player in Game.Controller.Players) 
				playerScores.Add (player.Name, player.CoinCount);
			return playerScores;
		}
	}

	/// <summary>
	/// Gets the top score. The key is the player's name.
	/// </summary>
	public KeyValuePair<string, int> TopScore {
		get { return Game.Score.PlayerScores.Aggregate ((l, r) => l.Value > r.Value ? l : r); }
	}

	/// <summary>
	/// Returns true if the player's score is large enough to be able to afford extra time.
	/// </summary>
	public bool CanAffordExtraTime {
		get { return PlayerScore >= Settings.ExtraTimeCost; }
	}

	/// <summary>
	/// Event that is called when the player's score is updated.
	/// </summary>
	public OnUpdateScore onUpdateScore;

	/// <summary>
	/// Event that is called when the pot's value is updated.
	/// </summary>
	public OnUpdatePot onUpdatePot;

	Settings settings;
	Settings Settings {
		get {
			if (settings == null) {
				settings = DataManager.GetSettings ();
			}
			return settings;
		}
	}

	public void Init () {
		Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
	}

	/// <summary>
	/// Sets the pot to its starting value
	/// </summary>
	public void FillPot () { Pot = Settings.PotCoinCount; }

	/// <summary>
	/// Sets the pot value to 0
	/// </summary>
	public void EmptyPot () { Pot = 0; }

	/// <summary>
	/// Adds coins to each player's score based on what round number this is and whether or not the player is the Decider
	/// </summary>
	public void AddRoundStartScores () {
		
		string deciderName = Game.Controller.DeciderName;
		int playerCoinCount;
		int deciderCoinCount;

		if (Game.Controller.RoundNumber == 0) {
			playerCoinCount = Settings.PlayerStartCoinCount;
			deciderCoinCount = Settings.DeciderStartCoinCount;
		} else {
			playerCoinCount = Settings.PlayerRoundStartCoinCount;
			deciderCoinCount = Settings.DeciderRoundStartCoinCount;
		}

		foreach (Player player in Game.Controller.Players) {
			if (player.Name != deciderName)
				player.CoinCount += playerCoinCount;
		}

		Game.Controller.Decider.CoinCount += deciderCoinCount;
		SendUpdateMessage ();
	}

	/// <summary>
	/// Adds coins to the winning player's score
	/// </summary>
	public void AddWinnings () {
		Game.Controller.Winner.CoinCount += Pot;
		EmptyPot ();
		SendUpdateMessage ();
	}

	/// <summary>
	/// Adds coins to the player's score based on the value defined in the agenda item
	/// </summary>
	public void ApplyPlayerReward (PlayerAgendaItem item) {
		Game.Controller.FindPlayer (item.PlayerName).CoinCount += Settings.Rewards[item.Reward];
		SendUpdateMessage ();
	}

	void AcceptExtraTime (NetMessage msg) {
		Pot += Settings.ExtraTimeCost;
		Game.Controller.FindPlayer (msg.str1).CoinCount -= Settings.ExtraTimeCost;
		SendUpdateMessage ();
	}

	void SendUpdateMessage () {
		if (onUpdateScore != null)
			onUpdateScore (PlayerScore);
	}

	void SendUpdatePotMessage () {
		if (onUpdatePot != null)
			onUpdatePot (Pot);
	}
}
