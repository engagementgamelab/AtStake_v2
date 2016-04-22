using UnityEngine;
using System.Collections;

namespace Views {

	public class FinalScoreboard : Scoreboard {

		protected override void OnInitDeciderElements () {}

		protected override void OnInitElements () {
			string winnerName = Game.Score.TopScore.Key;
			Elements.Add ("winning_player", new AvatarElement (winnerName, Game.Controller.GetAvatarForPlayer (winnerName), Game.Score.TopScore.Value));
			Elements.Add ("winner", new TextElement (Model.Text["winner"]));
			DisplayScores (false);
			Elements.Add ("menu", new ButtonElement (Model.Buttons["menu"], () => { 
				Game.EndGame ();
				GotoView ("start"); 
			}));
		}

		public override void OnDisconnect () {}
	}
}