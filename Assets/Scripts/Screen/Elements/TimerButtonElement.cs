#define FAST_TIME
using UnityEngine;

public class TimerButtonElement : ScreenElement<TimerButtonElementUI> {

	public float Duration {
		get { return duration; }
	}

	float duration;
	System.Action onStart;
	System.Action onEnd;

	public TimerButtonElement (float duration, System.Action onStart, System.Action onEnd=null) {
		this.duration = duration
		#if UNITY_EDITOR && FAST_TIME
		* 0.1f
		#endif
		;
		this.onStart = onStart;
		this.onEnd = onEnd;
	}

	protected override void OnRender (TimerButtonElementUI t) {
		t.RemoveButtonListeners ();
		t.AddButtonListener (StartTimer);
		Reset ();
	}

	protected override void OnRemove (TimerButtonElementUI t) {
		t.RemoveButtonListeners ();
	}

	public void StartTimer () {

		if (onStart != null)
			onStart ();

		uiElement.Interactable = false;
		Co.StartCoroutine (duration, OnUpdate, () => {
			if (onEnd != null)
				onEnd ();
		});
	}

	public void Reset (float newDuration=-1) {
		if (newDuration > -1) {
			duration = newDuration
			#if UNITY_EDITOR && FAST_TIME
			* 0.1f
			#endif
			;
		}
		uiElement.Interactable = true;
		OnUpdate (0);
	}

	void OnUpdate (float p) {
		float time = Mathf.Round (Mathf.Abs (p * duration - duration));
		uiElement.Text.text = time.ToString () + " seconds";
	}
}
