using UnityEngine;
using System.Collections;

public class ButtonElementUI : ScreenElementUI<ButtonElement> {

	public override void ApplyElement (ButtonElement e) {
		Text.text = e.Text;
		AddButtonListener (e.OnPress);
	}

	public override void RemoveElement (ButtonElement e) {
		RemoveButtonListeners ();
	}
}
