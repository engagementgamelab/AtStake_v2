using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Shows a list of games the player can join

	public class Games : View {

		protected override void OnInitElements () {
			Elements.Add ("back", new BackButtonElement ("hostjoin", () => { Game.Multiplayer.Disconnect (); }));
			Elements.Add ("game_list", new ListElement<ButtonElement> ());
			Elements.Add ("searching", new TextElement ("Searching for games..."));
		}

		protected override void OnShow () {

			ListElement<ButtonElement> list = GetScreenElement<ListElement<ButtonElement>> ("game_list");

			Game.Multiplayer.onRoomFull += OnRoomFull;
			Game.Multiplayer.onNameTaken += OnNameTaken;
			Game.Multiplayer.onConnect += OnConnect;

			Game.Multiplayer.RequestHostList ((List<string> hosts) => {
				Dictionary<string, ButtonElement> hostButtons = new Dictionary<string, ButtonElement> ();
				for (int i = 0; i < hosts.Count; i ++) {
					string hostId = hosts[i];
					hostButtons.Add (hostId, new ButtonElement (hostId, () => {
						Game.JoinGame (hostId);
					}));
				}
				list.Set (hostButtons);
				Elements["searching"].Active = list.Count == 0;
			});
		}

		protected override void OnHide () {
			Game.Multiplayer.onRoomFull = null;
			Game.Multiplayer.onNameTaken = null;
			Game.Multiplayer.onConnect = null;
		}
		
		void OnRoomFull () {
			Debug.Log ("ROOM FULL");
		}

		void OnNameTaken () {
			Debug.Log ("NAME TAKEN");
		}

		void OnConnect () { GotoView ("lobby"); }
	}
}