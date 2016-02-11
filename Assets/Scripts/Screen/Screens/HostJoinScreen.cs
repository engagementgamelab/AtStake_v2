using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This screens gives players the option to host a new game or join an existing one
// If hosting, the player is sent to the lobby to wait for players to connect
// If joining, the player is sent to a list of available games to join

public class HostJoinScreen : GameScreen {

	protected override void OnInitElements (Dictionary<string, ScreenElement> e) {
		e.Add ("host", new ButtonElement ("Host", Host));
		e.Add ("join", new ButtonElement ("Join", Join));
		e.Add ("back", new BackButtonElement ("name"));
	}

	void Host () {
		Game.HostGame ();
		GotoScreen ("lobby");
	}

	void Join () {
		GotoScreen ("games");
	}
}
