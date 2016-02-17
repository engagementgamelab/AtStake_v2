using UnityEngine;
using System.Collections;

// Displays instructions for the Decider to read out loud

public class ThinkInstructionsScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new NextButtonElement ("think"));
	}
}