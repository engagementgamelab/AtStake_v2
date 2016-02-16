using UnityEngine;
using System.Collections;

public class ScoreboardScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions", new TextElement ("When everyone is ready, press 'next' to start the next round"));
		Elements.Add ("next", new NextButtonElement ("roles"));
	}

	protected override void OnInitElements () {
		Elements.Add ("text", new TextElement ("At the end of round " + (Game.Rounds.Current+1) + " the scores are:"));
		DisplayScores ();
	}

	protected void DisplayScores () {
		foreach (var score in Game.Score.PlayerScores) {
			Elements.Add ("player_" + score.Key, new TextElement (score.Key + ": " + score.Value + " coins")); 
		}
	}
}
