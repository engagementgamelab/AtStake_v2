using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerElementUI : ScreenElementUI<TimerElement> {

	public Image timerBackground;
	public Image timerForeground;
	public Text headerText;
	public Text subheaderText;

	public override Text Text {
		get { return headerText; }
	}

	public override void ApplyElement (TimerElement e) {
		Text.text = e.Text;
		subheaderText.text = e.TimeText;
		timerForeground.fillAmount = e.Progress;
	}

	protected override void OnUpdate (TimerElement e) {
		Text.text = e.Text;
		subheaderText.text = e.TimeText;
		timerForeground.fillAmount = e.Progress;
	}
}
