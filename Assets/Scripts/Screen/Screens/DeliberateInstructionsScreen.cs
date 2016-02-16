using UnityEngine;
using System.Collections;

// Displays instructions for the Decider to read out loud

public class DeliberateInstructionsScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions", new TextElement ("read this out load: 'EVERYONE!!! we're gonna deliberate on who's got the best plan now'"));
		Elements.Add ("next", new NextButtonElement ("deliberate"));
	}

	protected override void OnInitElements () {
		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
	}
}