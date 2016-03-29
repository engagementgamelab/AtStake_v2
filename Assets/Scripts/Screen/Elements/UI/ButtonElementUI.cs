using UnityEngine;
using System.Collections;

public class ButtonElementUI : ScreenElementUI<ButtonElement> {

	public override void ApplyElement (ButtonElement e) {
		Text.text = e.Text;
		Text.ApplyStyle (TextStyle.Button);
		Interactable = e.Interactable;
		if (e.OnPress != null)
			AddButtonListener (e.OnPress);
		if (e.OnPressThis != null)
			AddButtonListener (() => { e.OnPressThis (e); });
	}

	protected override void OnUpdate (ButtonElement e) {
		Text.text = e.Text;
		Interactable = e.Interactable;
	}

	public override void RemoveElement (ButtonElement e) {
		RemoveButtonListeners ();
	}
}
