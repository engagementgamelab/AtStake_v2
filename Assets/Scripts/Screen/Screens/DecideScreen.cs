using UnityEngine;
using System.Collections;

public class DecideScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions", new TextElement ("Read aloud: 'i'm gonna choose a winner now :)'"));
		foreach (var peer in Game.Manager.Peers) {
			string name = peer.Key;
			Elements.Add ("peer_" + name, new ButtonElement (name, () => {
				Game.Dispatcher.ScheduleMessage ("ChooseWinner", name);
			}));
		}
	}

	protected override void OnShow () {
		Game.Dispatcher.AddListener ("ChooseWinner", ChooseWinner);
	}

	protected override void OnHide () {
		Game.Dispatcher.RemoveListener (ChooseWinner);
	}

	void ChooseWinner (NetworkMessage msg) {
		Game.Manager.Winner = msg.str1;
		GotoScreen ("winner");
	}
}