using UnityEngine;
using System.Collections;

public class PotElement : ScreenElement {

	string text;
	public string Text {
		get { return text; }
		set {
			text = value;
			SendUpdateMessage ();
		}
	}

	protected override void OnInit () {
		Game.Score.onUpdatePot += SetText;
		SetText (Game.Score.Pot);
	}

	void SetText (int pot) {
		Text = pot.ToString ();
	}
}