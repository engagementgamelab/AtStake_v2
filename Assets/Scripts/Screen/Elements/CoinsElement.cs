using UnityEngine;
using System.Collections;

public class CoinsElement : ScreenElement {

	string text;
	public string Text {
		get { return text; }
		set {
			text = value;
			SendUpdateMessage ();
		}
	}

	protected override void OnInit () {
		Game.Score.onUpdateScore += SetText;
		SetText (Game.Score.PlayerScore);
	}

	void SetText (int score) {
		Text = "Coins: " + score;
	}
}