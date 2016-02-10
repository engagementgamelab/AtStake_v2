using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This screens gives players the option to host a new game or join an existing one
// If hosting, the player is sent to the lobby to wait for players to connect
// If joining, the player is sent to a list of available games to join

public class DeckScreen : GameScreen {

	Dictionary<string, ScreenElement> elements;
	public override Dictionary<string, ScreenElement> Elements {
		get {
			if (elements == null) {
				elements = new Dictionary<string, ScreenElement> ();
			}
			return elements;
		}
	}

	protected override void OnShow () {
		AddElement ("instructions",
			new TextElement (IsHost ? "Please choose a deck genius" : "Please wait while the host chooses a deck"));
	}
}
