using UnityEngine;
using System.Collections;

public class TimerButtonElementUI : ScreenElementUI<TimerButtonElement> {

	public override void ApplyElement (TimerButtonElement e) {
		Text.text = e.Text;
		AddButtonListener (() => { 
			e.StartTimer (); 
		});
	}

	protected override void OnUpdate (TimerButtonElement e) {
		Interactable = e.Interactable;
		Text.text = e.Text;
	}

	public override void RemoveElement (TimerButtonElement e) {
		RemoveButtonListeners ();
	}
}
