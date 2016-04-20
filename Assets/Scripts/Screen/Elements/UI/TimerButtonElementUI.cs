using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerButtonElementUI : ScreenElementUI<TimerButtonElement> {

	public Image timerBackground;
	public Image timerForeground;
	public Text headerText;
	public Text subheaderText;
	public Button timerButton;

	public override Text Text {
		get { return headerText; }
	}

	public override Button Button {
		get { return timerButton; }
	}

	public static TextStyle TimerHeader {
		get {
			return new TextStyle () {
				FontSize = 32,
				FontStyle = FontStyle.Italic,
				FontColor = Palette.White,
				TextTransform = TextTransform.Lowercase
			};
		}
	}

	public static TextStyle TimerSubheader {
		get {
			return new TextStyle () {
				FontSize = 16,
				FontColor = Palette.White,
				TextTransform = TextTransform.Lowercase
			};
		}
	}

	public override void ApplyElement (TimerButtonElement e) {
		Text.text = e.Text;
		subheaderText.text = e.TimeText;
		timerForeground.fillAmount = e.Progress;
		AddButtonListener (() => { 
			e.StartTimer (); 
		});
		
		headerText.ApplyStyle (TimerHeader);
		subheaderText.ApplyStyle (TimerSubheader);
	}

	protected override void OnUpdate (TimerButtonElement e) {
		Interactable = e.Interactable;
		Text.text = e.Text;
		subheaderText.text = e.TimeText;
		timerForeground.fillAmount = e.Progress;

		headerText.ApplyStyle (TimerHeader);
		subheaderText.ApplyStyle (TimerSubheader);
	}

	public override void RemoveElement (TimerButtonElement e) {
		RemoveButtonListeners ();
	}
}
