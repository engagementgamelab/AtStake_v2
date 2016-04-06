using UnityEngine;
using System.Collections;

namespace Views {

	public class FinalScoreboard : Scoreboard {

		protected override void OnInitDeciderElements () {}

		protected override void OnInitElements () {
			Elements.Add ("scores", new TextElement (Model.Text["scores"]));
			DisplayScores ();
			Elements.Add ("next", new ButtonElement (Model.Buttons["menu"], () => { 
				Game.EndGame ();
				GotoView ("start"); 
			}));
		}

		public override void OnDisconnect () {}
	}
}