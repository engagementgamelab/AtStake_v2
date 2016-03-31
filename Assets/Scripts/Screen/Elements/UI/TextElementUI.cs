using UnityEngine;
using System.Collections;

public class TextElementUI : ScreenElementUI<TextElement> {

	public override void ApplyElement (TextElement e) {
		Text.text = e.Text;
		Text.ApplyStyle (Style);
	}

	protected override void OnUpdate (TextElement e) {
		Text.text = e.Text;
		if (Style != null)
			Text.ApplyStyle (Style);
	}
}
