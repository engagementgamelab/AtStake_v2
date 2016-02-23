using UnityEngine;

public class ButtonElement : ScreenElement {

	public readonly string Text;
	public readonly System.Action OnPress;

	public ButtonElement (string text, System.Action onPress) {
		Text = text;
		OnPress = onPress;
	}
}
