using UnityEngine;
using System.Collections;

public class ButtonElementUI : ScreenElementUI<ButtonElement> {

	public override void ApplyElement (ButtonElement e) {
		Text.text = e.text;
		AddButtonListener (e.onPress);
	}

	public override void RemoveElement (ButtonElement e) {
		RemoveButtonListeners ();
	}
}
