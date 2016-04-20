using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	public class Scoreboard : View {

		protected override void OnInitElements () {

			// Because the round has advanced by this point, the "previous decider" gets the "decider" content
			if (Game.Controller.PreviousDecider.Name == Name) {
				Elements.Add ("next", new NextButtonElement ("roles"));
				Elements.Add ("previous_decider_instructions", new TextElement (GetText ("previous_decider_instructions")));
			}

			Elements.Add ("round_end", new TextElement (
				DataManager.GetTextFromScreen (Model, "round_end", TextVariables)));

			DisplayScores ();
		}

		protected void DisplayScores () {
			
			Dictionary<string, TextElement> scores = Game.Score.PlayerScores
				.ToDictionary (x => x.Key, x => new TextElement (x.Key + ": " + x.Value + " coins"));

			Elements.Add ("score_list", new ListElement<TextElement> (scores));
		}
	}
}