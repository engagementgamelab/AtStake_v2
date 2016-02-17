using UnityEngine;
using System.Collections;

public class FinalScoreboardScreen : ScoreboardScreen {

	protected override void OnInitDeciderElements () {}

	protected override void OnInitElements () {
		Elements.Add ("scores", new TextElement (Model.Text["scores"]));
		DisplayScores ();
		Elements.Add ("menu", new ButtonElement (Model.Text["menu"], () => { 
			Game.EndGame ();
			GotoScreen ("start"); 
		}));
	}
}
