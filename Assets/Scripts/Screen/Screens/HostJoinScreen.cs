using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	}

	void Join () {
		game.Network.UpdateHosts ();
	}
}
