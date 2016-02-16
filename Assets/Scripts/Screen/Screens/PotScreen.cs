using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Explains how coins are distributed and what's """"@AT STAKE""""

public class PotScreen : GameScreen {

	protected override void OnInitElements () {
		Models.Settings settings = DataManager.GetSettings ();
		Elements.Add ("text1", new TextElement ("As the Decider, " + Game.Manager.Decider + " begins with " + settings.DeciderStartCoinCount + " coins."));
		Elements.Add ("text2", new TextElement ("Everyone else begins with " + settings.PlayerStartCoinCount + " coins."));
		Elements.Add ("text3", new TextElement ("At the end of the round, " + Game.Manager.Decider + " will choose a winner and they will get the coins in the pot. The pot has " + settings.PotCoinCount + " coins to start."));
	}

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new NextButtonElement ("bio"));
	}

	protected override void OnShow () {
		Game.Score.FillPot ();
		Game.Score.AddRoundStartScores ();
	}
}