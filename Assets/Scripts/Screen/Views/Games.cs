using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Shows a list of games the player can join

	public class Games : View {

		protected override void OnInitElements () {
			Elements.Add ("back", new BackButtonElement ("hostjoin", () => { Game.Multiplayer.Disconnect (); }));
			Elements.Add ("game_list", new ListElement<ButtonElement> ());
		}

		protected override void OnShow () {

			ListElement<ButtonElement> list = GetScreenElement<ListElement<ButtonElement>> ("game_list");

			Game.Multiplayer.RequestHostList ((List<string> hosts) => {
				Dictionary<string, ButtonElement> hostButtons = new Dictionary<string, ButtonElement> ();
				for (int i = 0; i < hosts.Count; i ++) {
					string hostId = hosts[i];
					hostButtons.Add (hostId, new ButtonElement (hostId, () => {
						Game.JoinGame (hostId, (int resultCode) => {
							if (resultCode == -1) {
								Debug.Log ("name taken");
							} else if (resultCode == -2) {
								Debug.Log ("room full");
							} else {
								GotoView ("lobby");
							}
						});
					}));
				}
				list.Set (hostButtons);
			});
		}
	}
}