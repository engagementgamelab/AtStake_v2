using UnityEngine;
using System.Collections;

public class TextElementUI : ScreenElementUI<TextElement> {

	public TextStyle Style { get; set; }

	public override void ApplyElement (TextElement e) {
		Text.text = e.Text;
	}

	protected override void OnUpdate (TextElement e) {
		Text.text = e.Text;
		if (Style != null)
			Text.ApplyStyle (Style);
	}
}
