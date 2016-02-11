using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The initial screen on loading the game

public class StartScreen : GameScreen {

	protected override void OnInitElements (Dictionary<string, ScreenElement> e) {
		e.Add ("text", new TextElement ("@Stake"));
		e.Add ("button", new ButtonElement ("Play", () => { GotoScreen ("name"); }));		
	}
}
