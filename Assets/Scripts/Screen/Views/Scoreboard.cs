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

		protected void DisplayScores () {
			Dictionary<string, AvatarElement> scores = Game.Score.PlayerScores
				.ToDictionary (x => x.Key, x => new AvatarElement (x.Key, Game.Controller.GetAvatarForPlayer (x.Key), x.Value));
			Elements.Add ("score_list", new ListElement<AvatarElement> (scores));
		}
	}
}