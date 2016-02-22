using UnityEngine;
using System.Collections;

namespace Views {

	public class Scoreboard : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("roles"));
		}

		protected override void OnInitElements () {
			Elements.Add ("round_end", new TextElement (
				DataManager.GetTextFromScreen (Model, "round_end", TextVariables)));
			DisplayScores ();
		}

		protected void DisplayScores () {
			foreach (var score in Game.Score.PlayerScores) {
				Elements.Add ("player_" + score.Key, new TextElement (score.Key + ": " + score.Value + " coins")); 
			}
		}
	}
}