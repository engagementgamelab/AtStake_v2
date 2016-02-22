using UnityEngine;
using UnityEngine.UI;

public class TextElement : ScreenElement<TextElementUI> {

	public readonly string text;
	public readonly TextStyle style;

	public TextElement (string text, TextStyle style=null) {
		this.text = text;
		this.style = (style == null) ? TextStyle.Paragraph : style;
	}
}
