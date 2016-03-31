using UnityEngine;
using System.Collections;

public class ButtonElementUI : ScreenElementUI<ButtonElement> {

	TextStyle style = TextStyle.Button;
	public TextStyle Style {
		get { return style; }
		set { style = value; }
	}

	public override Color Color {
		set { Image.color = value; }
	}

	public override void ApplyElement (ButtonElement e) {
		Text.text = e.Text;
		Text.ApplyStyle (Style);
		Interactable = e.Interactable;
		if (e.OnPress != null)
			AddButtonListener (e.OnPress);
		if (e.OnPressThis != null)
			AddButtonListener (() => { e.OnPressThis (e); });
	}

	protected override void OnUpdate (ButtonElement e) {
		Text.text = e.Text;
		Interactable = e.Interactable;
		Text.ApplyStyle (Style);
	}

	public override void RemoveElement (ButtonElement e) {
		RemoveButtonListeners ();
	}
}
