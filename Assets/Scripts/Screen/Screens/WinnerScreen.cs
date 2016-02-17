using UnityEngine;
using System.Collections;

public class WinnerScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new NextButtonElement ("agenda_item"));
		Game.Decks.ShuffleAgendaItems (Game.Manager.Peers);
		Game.Decks.NextAgendaItem ();
	}

	protected override void OnInitElements () {
		Elements.Add ("winner", new TextElement (DataManager.GetTextFromScreen (Model, "winner", TextVariables)));
	}

	protected override void OnShow () {
		Game.Score.AddWinnings ();
	}
}