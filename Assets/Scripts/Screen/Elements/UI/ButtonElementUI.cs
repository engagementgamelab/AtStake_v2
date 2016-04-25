using UnityEngine;
using System.Collections;

public class ButtonElementUI : ScreenElementUI<ButtonElement> {

	TextStyle style = TextStyle.LtButton;
	public override TextStyle Style {
		get { return style; }
		set { 
			style = value; 
			Text.ApplyStyle (style);
		}
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
		if (e.PlayClickSound != null)
			AddButtonListener (e.PlayClickSound);
	}

	protected override void OnUpdate (ButtonElement e) {
		Text.text = e.Text;
		Text.ApplyStyle (Style);
		Interactable = e.Interactable;
	}

	public override void RemoveElement (ButtonElement e) {
		RemoveButtonListeners ();
	}

	public void DelayedFadeIn (float delay=0.75f, float fadeTime=0.5f) {
		Alpha = 0f;
		Co.WaitForSeconds (delay, () => {
			Animate (new UIAnimator.FadeIn (fadeTime));
		});
	}
}
