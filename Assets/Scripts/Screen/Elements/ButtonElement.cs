using UnityEngine;

public class ButtonElement : ScreenElement {

	string text;
	public string Text {
		get { return text; }
		set {
			text = value;
			SendUpdateMessage ();
		}
	}

	bool interactable = true;
	public bool Interactable {
		get { return interactable; }
		set {
			interactable = value;
			SendUpdateMessage ();
		}
	}

	public readonly System.Action OnPress;
	public readonly System.Action<ButtonElement> OnPressThis;

	public ButtonElement (string text, System.Action onPress) {
		Text = text;
		OnPress = onPress;
	}

	public ButtonElement (string text, System.Action<ButtonElement> onPress) {
		Text = text;
		OnPressThis = onPress;
	}
}
