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
				elements.Add ("back", new BackButtonElement ("", () => { Game.Network.Disconnect (); }));
			}
			return elements;
		}
	}

	protected override void OnShow () {
		Game.Manager.onAddPeer += OnAddPeer;
		Game.Manager.onRemovePeer += OnRemovePeer;
	}

	protected override void OnHide () {
		Game.Manager.onAddPeer -= OnAddPeer;
		Game.Manager.onRemovePeer -= OnRemovePeer;
	}

	void OnAddPeer (string peer) {
		AddElement (peer, new TextElement (peer));
	}

	void OnRemovePeer (string peer) {
		RemoveElement (peer);
	}

	public override void OnDisconnect () {
		GoBack ();
	}
}
