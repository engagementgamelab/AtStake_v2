using UnityEngine;

public class TextElement : ScreenElement<TextElementUI> {

	string text;

	public TextElement (string text) {
		this.text = text;
	}

	protected override void OnRender (TextElementUI t) {
		t.Text.text = text;
	}

	public void SetText (string text) {
		this.text = text;
		uiElement.Text.text = text;
	}
}
