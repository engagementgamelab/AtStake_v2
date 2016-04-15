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

	public override void ApplyElement (TimerButtonElement e) {
		Text.text = e.Text;
		subheaderText.text = e.TimeText;
		timerForeground.fillAmount = e.Progress;
		AddButtonListener (() => { 
			e.StartTimer (); 
		});
		
		headerText.ApplyStyle (TextStyle.TimerHeader);
		subheaderText.ApplyStyle (TextStyle.TimerSubheader);
	}

	protected override void OnUpdate (TimerButtonElement e) {
		Interactable = e.Interactable;
		Text.text = e.Text;
		subheaderText.text = e.TimeText;
		timerForeground.fillAmount = e.Progress;

		headerText.ApplyStyle (TextStyle.TimerHeader);
		subheaderText.ApplyStyle (TextStyle.TimerSubheader);
	}

	public override void RemoveElement (TimerButtonElement e) {
		RemoveButtonListeners ();
	}
}
