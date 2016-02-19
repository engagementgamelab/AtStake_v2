using UnityEngine;
using System.Collections;

public class BackButtonElementUI : ScreenElementUI<BackButtonElement> {

	public override void ApplyElement (BackButtonElement e) {
		Text.text = e.text;
		AddButtonListener (e.onPress);
	}

	public override void RemoveElement (BackButtonElement e) {
		RemoveButtonListeners ();
	}
}
