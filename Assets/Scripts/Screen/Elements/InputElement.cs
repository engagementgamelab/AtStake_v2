using UnityEngine;

public class InputElement : ScreenElement {

	public readonly string Placeholder;
	public readonly System.Action<string> OnEndEdit;

	public InputElement (string placeholder, System.Action<string> onEndEdit) {
		Placeholder = placeholder;
		OnEndEdit = onEndEdit;
	}
}
