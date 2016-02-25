#define FAST_TIME
using UnityEngine;

public class TimerElement : ScreenElement {

	string text;
	public string Text {
		get { return text; }
		set {
			text = value;
			SendUpdateMessage ();
		}
	}

	float duration;
	System.Action onEnd;

	public TimerElement (float duration, System.Action onEnd=null) {
		this.duration = duration
		#if FAST_TIME
		* 0.1f
		#endif
		;
		this.onEnd = onEnd;
		Reset ();
	}

	public void StartTimer () {
		Co.StartCoroutine (duration, OnUpdateTime, () => {
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
		OnUpdateTime (0);
	}

	void OnUpdateTime (float p) {
		float time = Mathf.Round (Mathf.Abs (p * duration - duration));
		Text = time.ToString () + " seconds";
	}
}
