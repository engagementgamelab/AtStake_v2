using UnityEngine;
using System.Collections;

public class WinnerScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new ButtonElement ("Next", () => { AllGotoScreen ("agenda_item"); }));
		Game.Decks.ShuffleAgendaItems (Game.Manager.Peers);
		Game.Decks.NextAgendaItem ();
	}

	protected override void OnShow () {
		AddElement ("text", new TextElement ("And the winner is " + Game.Manager.Winner));
	}
}