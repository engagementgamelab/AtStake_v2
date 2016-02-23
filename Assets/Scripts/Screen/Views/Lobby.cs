﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Players wait in the lobby while other players connect to the game

	public class Lobby : View {

		ListElement<TextElement> peerList;

		protected override void OnInitElements () {
			Elements.Add ("back", new BackButtonElement ("", () => { Game.Multiplayer.Disconnect (); }));
			peerList = new ListElement<TextElement> ();
			Elements.Add ("peer_list", peerList);
		}

		protected override void OnShow () {

			OnAddPeer (Game.Manager.Player.Name);
			foreach (var peer in Game.Manager.Peers)
				OnAddPeer (peer.Key);

			Game.Manager.onAddPeer += OnAddPeer;
			Game.Manager.onRemovePeer += OnRemovePeer;
		}

		protected override void OnHide () {
			Game.Manager.onAddPeer -= OnAddPeer;
			Game.Manager.onRemovePeer -= OnRemovePeer;
		}

		void OnAddPeer (string peer) {
			// AddElement ("peer_" + peer, new TextElement (peer));
			Co.WaitForFixedUpdate (() => {
				peerList.Add (peer, new TextElement (peer));
				SetPlayButton ();
			});
		}

		void OnRemovePeer (string peer) {
			// RemoveElement ("peer_" + peer);
			peerList.Remove (peer);
			SetPlayButton ();
		}

		void SetPlayButton () {

			if (!IsHost) return;

			bool hasButton = HasElement ("play");
			bool hasMinPlayers = Game.Manager.Players.Count >= DataManager.GetSettings ().PlayerCountRange[0];

			if (hasMinPlayers && !hasButton) {
				AddElement ("play", new ButtonElement (Model.Buttons["play"], () => { AllGotoView ("deck"); }));
			} else if (hasButton && !hasMinPlayers) {
				RemoveElement ("play");
			}
		}

		public override void OnDisconnect () {
			GotoView ("hostjoin");
		}
	}
}