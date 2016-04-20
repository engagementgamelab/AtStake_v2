using UnityEngine;
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
		get { return Game.Controller.CoinCount; }
		set { 
			Game.Controller.CoinCount = value; 
			SendUpdateMessage ();
		}
	}
	
	public int Pot {
		get { return Game.Controller.Pot; }
		set { 
			Game.Controller.Pot = value; 
			SendUpdatePotMessage ();
		}
	}

	public Dictionary<string, int> PlayerScores {
		get {
			Dictionary<string, int> playerScores = new Dictionary<string, int> ();
			foreach (Player player in Game.Controller.Players) 
				playerScores.Add (player.Name, player.CoinCount);
			return playerScores;
		}
	}

	public bool CanAffordExtraTime {
		get { return PlayerScore >= Settings.ExtraTimeCost; }
	}

	public OnUpdateScore onUpdateScore;
	public OnUpdatePot onUpdatePot;

	public void Init () {
		Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
	}

	public void FillPot () { Pot = Settings.PotCoinCount; }
	public void EmptyPot () { Pot = 0; }

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

	public void AddWinnings () {
		Game.Controller.Winner.CoinCount += Pot;
		EmptyPot ();
		SendUpdateMessage ();
	}

	public void ApplyPlayerReward (PlayerAgendaItem item) {
		Game.Controller.FindPlayer (item.PlayerName).CoinCount += Settings.Rewards[item.Reward];
		SendUpdateMessage ();
	}

	void AcceptExtraTime (MasterMsgTypes.GenericMessage msg) {
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
