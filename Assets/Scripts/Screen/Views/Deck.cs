using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Deck selection screen. Host selects the deck while clients wait patiently.

	public class Deck : View {

		protected override void OnShow () {
			if (IsHost) {
				List<string> names = Game.Decks.Names;
				for (int i = 0; i < names.Count; i ++) {
					string name = names[i];
					AddElement ("deck_" + name, new ButtonElement (name, () => {
						Game.Dispatcher.ScheduleMessage ("SetDeck", name);
						AllGotoView ("roles");
					}));
				}
			}
		}
	}
}