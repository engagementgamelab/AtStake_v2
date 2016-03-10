using UnityEngine;
using System.Collections;

namespace Views {

	public class Winner : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("agenda_item"));
			Game.Decks.ShuffleAgendaItems (Game.Manager.Peers);
			Game.Decks.NextAgendaItem ();
		}

		protected override void OnInitElements () {
			Elements.Add ("winner", new TextElement (DataManager.GetTextFromScreen (Model, "winner")));
			Elements.Add ("winner_name", new TextElement (Game.Manager.Winner + "!", TextStyle.Header));
		}

		protected override void OnShow () {
			Game.Score.AddWinnings ();
		}
	}
}