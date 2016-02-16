﻿using UnityEngine;
using System.Collections;

// Displays instructions for the Decider to read out loud

public class ThinkInstructionsScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions", new TextElement ("read this out load: 'EVERYONE!!! we're gonna think for like 30 secs now'"));
		Elements.Add ("next", new NextButtonElement ("think"));
	}

	protected override void OnInitElements () {
		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
	}
}