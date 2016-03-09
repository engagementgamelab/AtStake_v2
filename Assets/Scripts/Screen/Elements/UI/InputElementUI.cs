using UnityEngine;
using System.Collections;

public class InputElementUI : ScreenElementUI<InputElement> {

	bool focusedOnLoad = false;

	public override void ApplyElement (InputElement e) {
		Placeholder.text = e.Placeholder;
		AddEndEditListener (e.OnEndEdit);
		if (e.OnValueChanged != null)
			AddValueChangedListener (e.OnValueChanged);
		focusedOnLoad = e.FocusedOnLoad;
	}

	public override void RemoveElement (InputElement e) {
		RemoveInputFieldListeners ();
	}

	protected override void OnInputEnabled (InputElement e) {
		if (focusedOnLoad)
			InputField.ActivateInputField ();
	}
}
