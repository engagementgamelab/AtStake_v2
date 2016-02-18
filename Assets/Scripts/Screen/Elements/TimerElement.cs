#define FAST_TIME
using UnityEngine;

public class TimerElement : ScreenElement<TimerElementUI> {

	float duration;
	System.Action onEnd;

	public TimerElement (float duration, System.Action onEnd=null) {
		this.duration = duration
		#if FAST_TIME
		* 0.1f
		#endif
		;
		this.onEnd = onEnd;
	}

	protected override void OnRender (TimerElementUI t) {
		t.Interactable = false;
		Reset ();
	}

	public void StartTimer () {
		Co.StartCoroutine (duration, OnUpdate, () => {
			if (onEnd != null)
				onEnd ();
		});
	}

	public void Reset (float newDuration=-1) {
		if (newDuration > -1) {
			duration = newDuration
			#if FAST_TIME
			* 0.1f
			#endif
			;
		}
		OnUpdate (0);
	}

	void OnUpdate (float p) {
		float time = Mathf.Round (Mathf.Abs (p * duration - duration));
		uiElement.Text.text = time.ToString () + " seconds";
	}
}
