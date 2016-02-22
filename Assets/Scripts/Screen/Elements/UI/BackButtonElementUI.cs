using UnityEngine;
using System.Collections;

public class BackButtonElementUI : ScreenElementUI<BackButtonElement> {

	public override void ApplyElement (BackButtonElement e) {
		Text.text = e.text;
		RemoveButtonListeners ();
		AddButtonListener (e.onPress);
	}
}
