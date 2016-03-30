﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Players wait in the lobby while other players connect to the game

	public class Lobby : View {

		ListElement<AvatarElement> peerList;

		protected override void OnInitElements () {

			peerList = new ListElement<AvatarElement> ();
			Elements.Add ("peer_list", peerList);

			Elements.Add ("back", new BackButtonElement ("", () => { Game.Multiplayer.Disconnect (); }));
			Elements.Add ("play", new ButtonElement (Model.Buttons["play"], 
				() => { AllGotoView ("deck"); }) { Active = false });
		}

		protected override void OnShow () {

			foreach (var player in Game.Manager.Players)
				OnAddPeer (player.Key, player.Value.Avatar);

			Game.Manager.onAddPeer += OnAddPeer;
			Game.Manager.onRemovePeer += OnRemovePeer;
		}

		protected override void OnHide () {
			Game.Manager.onAddPeer -= OnAddPeer;
			Game.Manager.onRemovePeer -= OnRemovePeer;
		}

		void OnAddPeer (string peer, string color) {
			peerList.Add (peer, new AvatarElement (peer, color));
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