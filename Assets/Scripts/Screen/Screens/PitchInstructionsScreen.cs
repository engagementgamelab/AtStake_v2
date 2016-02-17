using UnityEngine;
using System.Collections;

// Displays instructions for the Decider to read out loud

public class PitchInstructionsScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new NextButtonElement ("pitch"));
	}
}