using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// This screens gives players the option to host a new game or join an existing one
	// If hosting, the player is sent to the lobby to wait for players to connect
	// If joining, the player is sent to a list of available games to join

	public class HostJoin : View {

		protected override void OnInitElements () {
			Elements.Add ("host", new ButtonElement (Model.Buttons["host"], Host));
			Elements.Add ("join", new ButtonElement (Model.Buttons["join"], Join));
			Elements.Add ("back", new BackButtonElement ("start"));
			Elements.Add ("error", new TextElement (GetText ("error")) { Active = Game.Multiplayer.DisconnectedWithError });
		}

		void Host () {
			Game.StartGame ();
			Game.Multiplayer.HostGame ();
			GotoView ("lobby");
		}

		void Join () {
			GotoView ("games");
		}
	}
}