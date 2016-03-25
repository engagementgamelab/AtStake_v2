using UnityEngine;
using System.Collections;

namespace Views {

	public class Winner : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("agenda_item"));
		}

		protected override void OnInitElements () {
			Elements.Add ("winner", new TextElement (DataManager.GetTextFromScreen (Model, "winner")));
			Elements.Add ("winner_name", new TextElement (Game.Controller.Winner.Name + "!", TextStyle.Header));
		}

		protected override void OnShow () {
			Game.Score.AddWinnings ();
		}
	}
}