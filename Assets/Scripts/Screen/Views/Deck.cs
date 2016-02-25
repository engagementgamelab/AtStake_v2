using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Deck selection screen. Host selects the deck while clients wait patiently.

	public class Deck : View {

		protected override void OnInitHostElements () {
			Elements.Add ("deck_list", new ListElement<ButtonElement> ());
		}

		protected override void OnShow () {

			if (!IsHost) return;

			ListElement<ButtonElement> list = GetScreenElement<ListElement<ButtonElement>> ("deck_list");
			List<string> names = Game.Decks.Names;

			for (int i = 0; i < names.Count; i ++) {
				string name = names[i];
				list.Add (name, new ButtonElement (name, () => {
					Game.Dispatcher.ScheduleMessage ("SetDeck", name);
					AllGotoView ("roles");
				}));
			}
		}
	}
}