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

	// Color override ---- not needed?
	public readonly string AvatarColor;

	public TextElement (string text, string avatarColor="") {
		this.text = text;
		AvatarColor = avatarColor;
	}
}
