using UnityEngine;

public class InputElement : ScreenElement<InputElementUI> {

	string placeholder;

	public string Text {
		get { return ((InputElementUI)uiElement).Text.text; }
	}

	public InputElement (string placeholder) {
		this.placeholder = placeholder;
	}

	protected override void OnRender (InputElementUI i) {
		i.Placeholder.text = placeholder;
	}
}
