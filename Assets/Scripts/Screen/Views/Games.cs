using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Shows a list of games the player can join

	public class Games : View {

		protected override void OnInitElements () {
			Elements.Add ("back", new BackButtonElement ("hostjoin"));
			Elements.Add ("game_list", new ListElement<ButtonElement> ());
		}

		protected override void OnShow () {

			ListElement<ButtonElement> list = GetScreenElement<ListElement<ButtonElement>> ("game_list");

			// TODO: also listen to new games as they get added
			List<string> hosts = Game.Multiplayer.UpdateHosts ();
			for (int i = 0; i < hosts.Count; i ++) {
				string hostId = hosts[i];
				/*AddElement (i.ToString (), new ButtonElement (hostId, () => {
					Game.JoinGame (hostId);
					GotoView ("lobby");
				}));*/
				list.Add (hostId, new ButtonElement (hostId, () => {
					Game.JoinGame (hostId);
					GotoView ("lobby");
				}));
			}
		}
	}
}