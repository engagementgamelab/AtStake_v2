using UnityEngine;
using System.Collections;

public class WinnerScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new ButtonElement ("Next", () => {
			// AllGotoScreen ("agenda_point=");
		}));
	}

	protected override void OnShow () {
		Elements.Add ("text", new TextElement ("And the winner is " + Game.Manager.Winner));
	}
}