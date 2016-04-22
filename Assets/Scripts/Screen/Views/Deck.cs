using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Deck selection screen. Host selects the deck while clients wait patiently.

	public class Deck : View {

		protected override void OnInitHostElements () {
			Elements.Add ("deck_list", new RadioListElement (GetButton ("confirm"), (string selected) => {
				Game.Dispatcher.ScheduleMessage ("SetDeck", selected);
			}, Game.Decks.Names));
			Game.Dispatcher.AddListener ("SetDeck", OnSetDeck);
		}

		protected override void OnInitElements () {
			Elements.Add ("logo", new ImageElement ("logo"));
		}
		
		protected override void OnHide () {
			Game.Dispatcher.RemoveListener (OnSetDeck);
		}

		void OnSetDeck (NetMessage msg) {

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