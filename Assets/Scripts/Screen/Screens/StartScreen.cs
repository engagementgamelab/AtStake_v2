using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The initial screen on loading the game

public class StartScreen : GameScreen {

	protected override void OnInitElements () {
		Elements.Add ("play", new ButtonElement (Model.Buttons["play"], () => { GotoScreen ("name"); }));	
	}
}
