using UnityEngine;
using System.Collections;

public class InputElementUI : ScreenElementUI<InputElement> {

	public bool FocusedOnLoad { get; private set; }

	public override void ApplyElement (InputElement e) {
		Text.text = e.StartText;
		Placeholder.text = e.Placeholder;
		AddEndEditListener (e.OnEndEdit);
		if (e.OnValueChanged != null)
			AddValueChangedListener (e.OnValueChanged);
		FocusedOnLoad = e.FocusedOnLoad;
	}

	public override void RemoveElement (InputElement e) {
		RemoveInputFieldListeners ();
	}

	protected override void OnInputEnabled (InputElement e) {
		if (FocusedOnLoad)
			InputField.ActivateInputField ();
	}
}
