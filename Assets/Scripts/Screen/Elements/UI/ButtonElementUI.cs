using UnityEngine;
using System.Collections;

public class ButtonElementUI : ScreenElementUI<ButtonElement> {

	public override void ApplyElement (ButtonElement e) {
		Text.text = e.Text;
		Interactable = e.Interactable;
		AddButtonListener (e.OnPress);
	}

	protected override void OnUpdate (ButtonElement e) {
		Text.text = e.Text;
		Interactable = e.Interactable;
	}

	public override void RemoveElement (ButtonElement e) {
		RemoveButtonListeners ();
	}
}
