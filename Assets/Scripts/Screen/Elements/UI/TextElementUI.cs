using UnityEngine;
using System.Collections;

public class TextElementUI : ScreenElementUI<TextElement> {

	public override void ApplyElement (TextElement e) {
		Text.text = e.Text;
		Text.fontStyle = e.Style.FontStyle;
		Text.color = e.Style.FontColor;
		Text.alignment = e.Style.TextAnchor;
		Text.fontSize = e.Style.FontSize;
	}

	// TODO: UpdateText/SetText
}
