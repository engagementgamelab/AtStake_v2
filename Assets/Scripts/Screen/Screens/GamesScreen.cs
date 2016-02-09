using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Shows a list of games the player can join

public class GamesScreen : GameScreen {

	Dictionary<string, ScreenElement> elements;
	public override Dictionary<string, ScreenElement> Elements {
		get {
			if (elements == null) {
				elements = new Dictionary<string, ScreenElement> ();
				elements.Add ("text", new TextElement ("Select a game to join"));
				elements.Add ("back", new BackButtonElement ("hostjoin"));
			}
			return elements;
		}
	}

	protected override void OnShow () {
		// TODO: also listen to new games as they get added
		List<string> hosts = game.Network.UpdateHosts ();
		for (int i = 0; i < hosts.Count; i ++) {
			string hostId = hosts[i];
			AddElement (i.ToString (), new ButtonElement (hostId, () => {
				game.JoinGame (hostId);
				GotoScreen ("lobby");
			}));
		}
	}
}
