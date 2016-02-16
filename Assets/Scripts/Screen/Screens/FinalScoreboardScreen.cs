using UnityEngine;
using System.Collections;

public class FinalScoreboardScreen : ScoreboardScreen {

	protected override void OnInitDeciderElements () {}

	protected override void OnInitElements () {
		Elements.Add ("text", new TextElement ("And the final scores are:"));
		DisplayScores ();
		Elements.Add ("menu", new ButtonElement ("Main menu", () => { 
			Game.EndGame ();
			GotoScreen ("start"); 
		}));
	}
}
