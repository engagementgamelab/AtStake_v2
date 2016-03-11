using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Explains how coins are distributed and what's """"@AT STAKE""""

	public class Pot : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("bio"));
		}

		protected override void OnInitElements () {
			Elements.Add ("instruction1", new TextElement (GetText ("instruction1")));
			Elements.Add ("instruction2", new TextElement (GetText ("instruction2")));
			Elements.Add ("instruction3", new TextElement (GetText ("instruction3")));
			Elements.Add ("instruction4", new TextElement (GetText ("instruction4")));
			Elements.Add ("instruction5", new TextElement (GetText ("instruction5")));
		}

		protected override void OnShow () {
			Game.Score.FillPot ();
			Game.Score.AddRoundStartScores ();
		}
	}
}