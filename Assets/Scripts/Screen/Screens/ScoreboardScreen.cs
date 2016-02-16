using UnityEngine;
using System.Collections;

public class ScoreboardScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions", new TextElement ("When everyone is ready, press 'next' to start the next round"));
		Elements.Add ("next", new ButtonElement ("Next", () => { AllGotoScreen ("roles"); }));
	}

	protected override void OnInitElements () {
		Elements.Add ("text", new TextElement ("At the end of round " + (Game.Rounds.Current+1) + " the scores are:"));
		DisplayScores ();
	}

	protected void DisplayScores () {
		foreach (var player in Game.Manager.Players) {
			Elements.Add ("player_" + player.Key, new TextElement (player.Key + ": " + player.Value.CoinCount + " coins")); 
		}
	}
}
