using UnityEngine;

public class ButtonElement : ScreenElement<ButtonElementUI> {

	public readonly string text;
	public readonly System.Action onPress;

	public ButtonElement (string text, System.Action onPress) {
		this.text = text;
		this.onPress = onPress;
	}

	// TODO: have ScreenElementUI handle this
	protected override void OnRender (ButtonElementUI b) {
		/*b.Text.text = text;
		b.RemoveButtonListeners ();
		b.AddButtonListener (onPress);*/
	}

	// TODO: have ScreenElementUI handle this	
	protected override void OnRemove (ButtonElementUI b) {
		// b.RemoveButtonListeners ();
	}
}
