using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Players wait in the lobby while other players connect to the game

	public class Lobby : View {

		ListElement<TextElement> peerList;

		protected override void OnInitElements () {

			peerList = new ListElement<TextElement> ();
			Elements.Add ("peer_list", peerList);

			Elements.Add ("back", new BackButtonElement ("", () => { Game.Multiplayer.Disconnect (); }));
			Elements.Add ("play", new ButtonElement (Model.Buttons["play"], 
				() => { AllGotoView ("deck"); }) { Active = false });
		}

		protected override void OnShow () {

			foreach (string player in Game.Manager.PlayerNames)
				OnAddPeer (player);

			Game.Manager.onAddPeer += OnAddPeer;
			Game.Manager.onRemovePeer += OnRemovePeer;
		}

		protected override void OnHide () {
			Game.Manager.onAddPeer -= OnAddPeer;
			Game.Manager.onRemovePeer -= OnRemovePeer;
		}

		void OnAddPeer (string peer) {
			peerList.Add (peer, new TextElement (peer));
			SetPlayButton ();
		}

		void OnRemovePeer (string peer) {
			peerList.Remove (peer);
			SetPlayButton ();
		}

		void SetPlayButton () {
			if (IsHost)
				Elements["play"].Active = Game.Manager.Players.Count >= DataManager.GetSettings ().PlayerCountRange[0];
		}

		public override void OnDisconnect () {
			GotoView ("hostjoin");
		}
	}
}