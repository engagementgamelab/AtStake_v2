using UnityEngine;

public class ButtonElement : ScreenElement<ButtonElementUI> {

	public readonly string text;
	public readonly System.Action onPress;

	public ButtonElement (string text, System.Action onPress) {
		this.text = text;
		this.onPress = onPress;
	}
}
