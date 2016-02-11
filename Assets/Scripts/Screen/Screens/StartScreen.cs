using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The initial screen on loading the game

public class StartScreen : GameScreen {

	protected override void OnInitElements () {
		Elements.Add ("text", new TextElement ("@Stake"));
		Elements.Add ("button", new ButtonElement ("Play", () => { GotoScreen ("name"); }));		
	}
}
