using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Shows a list of games the player can join

	public class Games : View {

		protected override void OnInitElements () {
			Elements.Add ("back", new BackButtonElement ("hostjoin", () => { Game.Multiplayer.Disconnect (); }));
			Elements.Add ("searching", new TextElement (GetText ("searching")));
			Elements.Add ("logo", new ImageElement ("logo"));
			Elements.Add ("game_list", new RadioListElement (GetButton ("confirm"), (string selected) => {
				JoinGame (selected);
			}));
		}

		protected override void OnShow () {

			RadioListElement list = GetScreenElement<RadioListElement> ("game_list");

			Game.Multiplayer.RequestHostList ((List<string> hosts) => {

				if (hosts.Contains ("__error")) {
					GetScreenElement<TextElement> ("searching").Text = GetText ("error");
				} else {
					GetScreenElement<TextElement> ("searching").Text = GetText ("searching");
					list.Set (hosts);
					Elements["searching"].Active = hosts.Count == 0;
				}
			});
		}

		void JoinGame (string hostId) {
			Game.StartGame ();
			Game.Multiplayer.JoinGame (hostId, (ResponseType res) => {
				switch (res) {
					case ResponseType.Success: GotoView ("lobby"); break;
					case ResponseType.NameTaken:
						Game.EndGame ();
						break;
				}
			});
		}

		public override void OnDisconnect () {
			Debug.LogWarning ("Name taken!");
		}
	}
}