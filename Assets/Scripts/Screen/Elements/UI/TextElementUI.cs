using UnityEngine;
using System.Collections;

public class TextElementUI : ScreenElementUI<TextElement> {

	TextStyle style;
	public TextStyle Style {
		get { return style; }
		set {
			style = value;
			Text.ApplyStyle (style);
		}
	}

	public override void ApplyElement (TextElement e) {
		Text.text = e.Text;
	}

	protected override void OnUpdate (TextElement e) {
		Text.text = e.Text;
		if (Style != null)
			Text.ApplyStyle (Style);
	}
}
