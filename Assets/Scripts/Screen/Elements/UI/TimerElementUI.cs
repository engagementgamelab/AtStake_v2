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

	public static TextStyle TimerHeader {
		get {
			return new TextStyle () {
				FontSize = 22,
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

	public override void ApplyElement (TimerElement e) {
		Text.text = e.Text;
		subheaderText.text = e.TimeText;
		timerForeground.fillAmount = e.Progress;
		headerText.ApplyStyle (TimerHeader);
		subheaderText.ApplyStyle (TimerSubheader);
	}

	protected override void OnUpdate (TimerElement e) {
		Text.text = e.Text;
		subheaderText.text = e.TimeText;
		timerForeground.fillAmount = e.Progress;
		headerText.ApplyStyle (TimerHeader);
		subheaderText.ApplyStyle (TimerSubheader);
	}
}
