using UnityEngine;

public class InputElement : ScreenElement {

	public readonly string Placeholder;
	public readonly System.Action<string> OnValueChanged;
	public readonly System.Action<string> OnEndEdit;

	public bool FocusedOnLoad { get; set; }

	public InputElement (string placeholder, System.Action<string> onEndEdit) {
		Placeholder = placeholder;
		OnEndEdit = onEndEdit;
	}

	public InputElement (string placeholder, System.Action<string> onValueChanged, System.Action<string> onEndEdit) {
		Placeholder = placeholder;
		OnValueChanged = onValueChanged;
		OnEndEdit = onEndEdit;
	}
}
