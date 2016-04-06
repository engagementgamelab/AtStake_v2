using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Shows a list of games the player can join

	public class Games : View {

		string selectedGame = "";

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
			Game.Multiplayer.JoinGame (hostId, (string response) => {
				Game.StartGame ();
				switch (response) {
					case "room_full": 
						Debug.Log ("ROOM FULL");
						// OnShow (); 
						break;
					case "name_taken":
						Debug.Log ("NAME TAKEN");
						// todo
						// OnShow ();
						break;
					case "registered":
						GotoView ("lobby");
						break;
				}
			});
		}
	}
}