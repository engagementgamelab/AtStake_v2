using UnityEngine;
using System.Collections;

// Shows the question that players will be debating in this round
// The Decider is instructed to read a script

public class QuestionScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions1", new TextElement ("read this out loud: 'yo everyone listen to this question'"));
		Elements.Add ("question", new TextElement (Game.Decks.GetQuestion ()));
		Elements.Add ("instructions2", new TextElement ("when everyne is done press next"));
		Elements.Add ("next", new NextButtonElement ("think_instructions"));
	}

	protected override void OnInitPlayerElements () {
		Elements.Add ("question", new TextElement (Game.Decks.GetQuestion ()));
	}

	protected override void OnInitElements () {
		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
	}
}