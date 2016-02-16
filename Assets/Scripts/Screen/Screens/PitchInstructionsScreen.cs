using UnityEngine;
using System.Collections;

// Displays instructions for the Decider to read out loud

public class PitchInstructionsScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions", new TextElement ("read this out load: 'EVERYONE!!! we're gonna take turns pitching now'"));
		Elements.Add ("next", new ButtonElement ("Next", () => { AllGotoScreen ("pitch"); }));
	}

	protected override void OnInitElements () {
		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
	}
}