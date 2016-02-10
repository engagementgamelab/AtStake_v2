using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Players wait in the lobby while other players connect to the game

public class LobbyScreen : GameScreen {

	Dictionary<string, ScreenElement> elements;
	public override Dictionary<string, ScreenElement> Elements {
		get {
			if (elements == null) {
				elements = new Dictionary<string, ScreenElement> ();
				elements.Add ("text", new TextElement ("Lobby"));
				elements.Add ("back", new BackButtonElement ("", () => { Game.Multiplayer.Disconnect (); }));
			}
			return elements;
		}
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
		AddElement ("peer_" + peer, new TextElement (peer));
		SetPlayButton ();
	}

	void OnRemovePeer (string peer) {
		RemoveElement ("peer_" + peer);
		SetPlayButton ();
	}

	void SetPlayButton () {

		if (!IsHost) return;

		bool hasButton = HasElement ("play");
		bool hasMinPlayers = Game.Manager.Players.Count >= 3; // TODO: get minimum player count from settings (Models.Settings)

		if (hasMinPlayers && !hasButton) {
			AddElement ("play", new ButtonElement ("Play", () => { AllGotoScreen ("deck"); }));
		} else if (hasButton && !hasMinPlayers) {
			RemoveElement ("play");
		}
	}

	public override void OnDisconnect () {
		GotoScreen ("hostjoin");
	}
}
