using UnityEngine;
using System.Collections;

public class TextElementUI : ScreenElementUI<TextElement> {

	public override void ApplyElement (TextElement e) {
		Text.text = e.text;
		Text.fontStyle = e.style.FontStyle;
		Text.color = e.style.FontColor;
		Text.alignment = e.style.TextAnchor;
		Text.fontSize = e.style.FontSize;
	}

	public override void RemoveElement (TextElement e) {
		Text.text = "";
	}

	// TODO: UpdateText/SetText
}
