using UnityEngine;
using System.Collections;

public class TimerElementUI : ScreenElementUI<TimerElement> {

	public override void ApplyElement (TimerElement e) {
		Interactable = false;
		e.onUpdate += OnUpdate;
		Text.text = e.Text;
	}

	void OnUpdate (TimerElement e) {
		Text.text = e.Text;
	}
}
