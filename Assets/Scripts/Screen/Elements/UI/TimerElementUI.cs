using UnityEngine;
using System.Collections;

public class TimerElementUI : ScreenElementUI<TimerElement> {

	public override void ApplyElement (TimerElement e) {
		Interactable = false;
		Text.text = e.Text;
	}

	protected override void OnUpdate (TimerElement e) {
		Text.text = e.Text;
	}
}
