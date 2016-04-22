using UnityEngine;

public class InputElement : ScreenElement {

	public readonly string StartText;
	public readonly string Placeholder;
	public readonly System.Action<string> OnValueChanged;
	public readonly System.Action<string> OnEndEdit;

	public bool FocusedOnLoad { get; set; }

	public InputElement (string startText, string placeholder, System.Action<string> onEndEdit) {
		StartText = startText;
		Placeholder = placeholder;
		OnEndEdit = onEndEdit;
	}

	public InputElement (string startText, string placeholder, System.Action<string> onValueChanged, System.Action<string> onEndEdit) {
		StartText = startText;
		Placeholder = placeholder;
		OnValueChanged = onValueChanged;
		OnEndEdit = onEndEdit;
	}
}
