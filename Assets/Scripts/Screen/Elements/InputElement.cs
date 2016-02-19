using UnityEngine;

public class InputElement : ScreenElement<InputElementUI> {

	public readonly string placeholder;
	public readonly System.Action<string> onEndEdit;

	public string Text {
		get { return ((InputElementUI)uiElement).Text.text; }
	}

	public InputElement (string placeholder, System.Action<string> onEndEdit) {
		this.placeholder = placeholder;
		this.onEndEdit = onEndEdit;
	}

	protected override void OnRender (InputElementUI i) {
		// i.Placeholder.text = placeholder;
	}
}
