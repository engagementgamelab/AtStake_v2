using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Shows a list of games the player can join

public class GamesScreen : GameScreen {

	protected override void OnInitElements () {
		Elements.Add ("back", new BackButtonElement ("hostjoin"));
	}

	protected override void OnShow () {
		// TODO: also listen to new games as they get added
		List<string> hosts = Game.Multiplayer.UpdateHosts ();
		for (int i = 0; i < hosts.Count; i ++) {
			string hostId = hosts[i];
			AddElement (i.ToString (), new ButtonElement (hostId, () => {
				Game.JoinGame (hostId);
				GotoScreen ("lobby");
			}));
		}
	}
}
