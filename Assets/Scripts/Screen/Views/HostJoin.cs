using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// This screens gives players the option to host a new game or join an existing one
	// If hosting, the player is sent to the lobby to wait for players to connect
	// If joining, the player is sent to a list of available games to join

	public class HostJoin : View {

		protected override void OnInitElements () {
			Elements.Add ("host", new ButtonElement (GetButton ("host"), Host));
			Elements.Add ("join", new ButtonElement (GetButton ("join"), Join));
			Elements.Add ("back", new BackButtonElement ("start"));
			Elements.Add ("logo", new ImageElement ("logo"));
			Elements.Add ("welcome", new TextElement (GetText ("welcome")));
			Elements.Add ("error", new TextElement (GetText ("error")) { Active = Game.Multiplayer.DisconnectedWithError });
		}

		void Host () {

			#if UNITY_EDITOR && SINGLE_SCREEN
			if (UnityEngine.Networking.NetworkServer.active) {
				Debug.LogWarning ("Only one host is allowed in Single Screen mode. Please choose 'Join.'");
				return;
			}
			#endif

			Game.StartGame ();
			Game.Multiplayer.HostGame ((ResponseType res) => {
				
				// TODO: feedback for when name is taken (ResponseType.NameTaken)
				if (res == ResponseType.Success) {
					GotoView ("lobby");
				}
			});
		}

		void Join () {
			GotoView ("games");
		}
	}
}