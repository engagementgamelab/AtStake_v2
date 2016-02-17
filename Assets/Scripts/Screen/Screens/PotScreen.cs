using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Explains how coins are distributed and what's """"@AT STAKE""""

public class PotScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new NextButtonElement ("bio"));
	}

	protected override void OnShow () {
		Game.Score.FillPot ();
		Game.Score.AddRoundStartScores ();
	}
}