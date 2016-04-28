using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// If the player tried to join a game that contained a player with the same name, this screen gives them a chance to submit a new name

	public class NameTaken : Start {

		protected override void OnInitElements () {

			Elements.Add ("back", new BackButtonElement ("start"));
			Elements.Add ("connection_failed", new TextElement (GetText ("connection_failed")) { Active = !Connected });

			submitButton = new ButtonElement (GetButton ("submit"), () => { 
				if (Game.Manager.Name != "" && Game.Manager.Name != Game.Multiplayer.TakenName && Connected)
					SubmitName ();
			}) {
				#if !SINGLE_SCREEN
				Interactable = false 
				#endif
			};

			Elements.Add ("submit", submitButton);

			Elements.Add ("input", new InputElement (Game.Multiplayer.TakenName, "your name", (string name) => {
				#if !SINGLE_SCREEN
				submitButton.Interactable = name != "" && name != Game.Multiplayer.TakenName && Connected;
				#endif
			}, (string name) => {
				Game.Manager.Name = name;

				// This allows the name to be submitted by pressing "done" on the ios/android keyboard
				if (name != "" && name != Game.Multiplayer.TakenName && Connected)
					SubmitName ();
			}));
		}

		void SubmitName () {
			if (string.IsNullOrEmpty (Game.Multiplayer.AttemptedHost)) {
				HostGame ();
			} else {
				JoinGame (Game.Multiplayer.AttemptedHost);
			}
		}
	}
}