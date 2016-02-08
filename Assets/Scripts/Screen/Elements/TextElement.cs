using UnityEngine;

public class TextElement : ScreenElement<TextElementUI> {

	TextElementUI ui;
	string text;

	public string Text {
		get { return text; }
	}

	public TextElement (string text) {
		this.text = text;
	}

	protected override void OnRender (TextElementUI t) {
		t.Text.text = Text;
	}
}
