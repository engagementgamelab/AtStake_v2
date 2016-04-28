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

		protected override void OnNameTaken () {
			if (!string.IsNullOrEmpty (Game.Multiplayer.TakenName))
				GotoView ("name_taken");
		}

		public override void OnDisconnect () {}
	}
}