using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Explains how coins are distributed and what's """"@AT STAKE""""

	public class Pot : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("bio"));
		}

		protected override void OnShow () {
			Game.Score.FillPot ();
			Game.Score.AddRoundStartScores ();
		}
	}
}