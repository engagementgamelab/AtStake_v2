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
	public System.Action PlayClickSound { get; private set; }

	public ButtonElement (string text, System.Action onPress, string clickSound="click1") {
		Text = text;
		OnPress = onPress;
		SetClickSound (clickSound);
	}

	public ButtonElement (string text, System.Action<ButtonElement> onPress, string clickSound="click1") {
		Text = text;
		OnPressThis = onPress;
		SetClickSound (clickSound);
	}

	public void TestPress () {
		if (OnPress != null)
			OnPress ();
		if (OnPressThis != null)
			OnPressThis (this);
	}

	void SetClickSound (string clickSound) {
		audioClip = clickSound;
		if (clickSound == "")
			PlayClickSound = null;
		else
			PlayClickSound += PlayAudio;
	}
}
