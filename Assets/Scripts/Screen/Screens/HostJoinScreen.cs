using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This screens gives players the option to host a new game or join an existing one
// If hosting, the player is sent to the lobby to wait for players to connect
// If joining, the player is sent to a list of available games to join

public class HostJoinScreen : GameScreen {

	Dictionary<string, ScreenElement> elements;
	public override Dictionary<string, ScreenElement> Elements {
		get {
			if (elements == null) {
				elements = new Dictionary<string, ScreenElement> ();
				elements.Add ("host", new ButtonElement ("Host", Host));
				elements.Add ("join", new ButtonElement ("Join", Join));
				elements.Add ("back", new BackButtonElement ("name"));
			}
			return elements;
		}
	}

	void Host () {
		game.Network.HostGame ();
		GotoScreen ("lobby");
	}

	void Join () {
		GotoScreen ("games");
	}
}
