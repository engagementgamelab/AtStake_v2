using UnityEngine;
using UnityEngine.UI;

public class TextElement : ScreenElement<TextElementUI> {

	public readonly string text;
	public readonly TextStyle style;

	public TextElement (string text, TextStyle style=null) {
		this.text = text;
		this.style = (style == null) ? TextStyle.Paragraph : style;
	}

	protected override void OnRender (TextElementUI t) {
		/*Text el = t.Text;
		el.text = text;
		el.fontStyle = style.FontStyle;
		el.color = style.FontColor;
		el.alignment = style.TextAnchor;
		el.fontSize = style.FontSize;*/
	}

	public void SetText (string text) {
		/*this.text = text;
		uiElement.Text.text = text;*/
	}
}
