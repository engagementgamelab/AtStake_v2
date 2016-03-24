﻿using UnityEngine;
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

			Game.Dispatcher.AddListener ("SetDeck", OnSetDeck);

			ListElement<ButtonElement> list = GetScreenElement<ListElement<ButtonElement>> ("deck_list");
			List<string> names = Game.Decks.Names;

			for (int i = 0; i < names.Count; i ++) {
				string name = names[i];
				list.Add (name, new ButtonElement (name, () => {
					Game.Dispatcher.ScheduleMessage ("SetDeck", name);
				}));
			}
		}

		protected override void OnHide () {
			Game.Dispatcher.RemoveListener (OnSetDeck);
		}

		void OnSetDeck (MasterMsgTypes.GenericMessage msg) {

			// Wait a frame to ensure that DeckManager sets the deck before referencing it
			Co.WaitForFixedUpdate (() => {
				Game.Dispatcher.ScheduleMessage ("StartGame");
				Co.YieldWhileTrue (() => { return !Game.Controller.DataLoaded; }, () => {
					AllGotoView ("roles");
				});
			});
		}
	}
}