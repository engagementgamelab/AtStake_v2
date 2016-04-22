using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	public class Scoreboard : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next_round", new ButtonElement ("Next", () => { 
				if (Game.Controller.NextRound ()) {
					AllGotoView ("roles");
				}
			}));
		}

		protected override void OnInitElements () {
			
			Elements.Add ("round_end", new TextElement (
				DataManager.GetTextFromScreen (Model, "round_end", TextVariables)));

			DisplayScores ();
		}

		protected void DisplayScores (bool includeTopScore=true) {

			Dictionary<string, int> playerScores = Game.Score.PlayerScores;
			if (!includeTopScore)
				playerScores.Remove (Game.Score.TopScore.Key);

			Dictionary<string, AvatarElement> scores = playerScores.ToDictionary (x => x.Key, x => new AvatarElement (x.Key, Game.Controller.GetAvatarForPlayer (x.Key), x.Value));
			Elements.Add ("score_list", new ListElement<AvatarElement> (scores));
		}
	}
}