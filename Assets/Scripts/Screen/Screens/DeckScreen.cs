using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Deck selection screen. Host selects the deck while clients wait patiently.

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
