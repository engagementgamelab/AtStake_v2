using UnityEngine;
using System.Collections;

public class PotElement : ScreenElement<PotElementUI> {

	protected override void OnRender (PotElementUI p) {
		SetText ();
		Game.Score.Inventory["pot"].onUpdate += SetText;
	}

	protected override void OnRemove (PotElementUI p) {
		Game.Score.Inventory["pot"].onUpdate -= SetText;
	}

	void SetText () {
		uiElement.Text.text = "Pot: " + Game.Score.Pot;
	}
}