using UnityEngine;
using UnityEngine.UI;

public class TextElement : ScreenElement {

	string text;
	public string Text {
		get { return text; }
		set {
			text = value;
			SendUpdateMessage ();
		}
	}

	public readonly TextStyle Style;

	public TextElement (string text, TextStyle style=null) {
		this.text = text;
		Style = (style == null) ? TextStyle.Paragraph : style;
	}
}
