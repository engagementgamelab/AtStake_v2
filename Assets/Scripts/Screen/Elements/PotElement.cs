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
		Game.Score.Inventory["pot"].onUpdate += SetText;
		SetText ();
	}

	void SetText () {
		Text = "Pot: " + Game.Score.Pot;
	}
}