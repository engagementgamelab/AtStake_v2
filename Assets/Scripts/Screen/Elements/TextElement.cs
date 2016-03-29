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

	public TextElement (string text) {
		this.text = text;
	}
}
