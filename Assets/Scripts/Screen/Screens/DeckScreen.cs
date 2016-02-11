using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Deck selection screen. Host selects the deck while clients wait patiently.

public class DeckScreen : GameScreen {

	protected override void OnShow () {
		if (IsHost) {
			AddElement ("instructions", new TextElement ("Please choose a deck genius"));
			List<string> names = Game.Decks.Names;
			for (int i = 0; i < names.Count; i ++) {
				string name = names[i];
				AddElement ("deck_" + name, new ButtonElement (name, () => {
					Game.Dispatcher.ScheduleMessage ("SetDeck", name);
					AllGotoScreen ("roles");
				}));
			}
		} else {
			AddElement ("instructions", new TextElement("Please wait while the host chooses a deck"));
		}
	}
}
