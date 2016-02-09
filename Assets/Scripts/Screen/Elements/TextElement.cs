using UnityEngine;

public class TextElement : ScreenElement<TextElementUI> {

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
