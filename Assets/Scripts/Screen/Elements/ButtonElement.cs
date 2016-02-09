using UnityEngine;

public class ButtonElement : ScreenElement<ButtonElementUI> {

	string text;
	System.Action onPress;

	public ButtonElement (string text, System.Action onPress) {
		this.text = text;
		this.onPress = onPress;
	}

	protected override void OnRender (ButtonElementUI b) {
		b.Text.text = text;
		b.AddButtonListener (onPress);
	}

	protected override void OnRemove (ButtonElementUI b) {
		b.RemoveButtonListeners ();
	}
}
