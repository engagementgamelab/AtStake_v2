#define FAST_TIME
using UnityEngine;

public class TimerButtonElement : ScreenElement {

	string text;
	public string Text {
		get { return text; }
		set {
			text = value;
			SendUpdateMessage ();
		}
	}

	bool interactable;
	public bool Interactable {
		get { return interactable; }
		private set {
			interactable = value;
			SendUpdateMessage ();
		}
	}

	public float Duration {
		get { return duration; }
	}

	float duration;
	System.Action onStart;
	System.Action onEnd;

	public TimerButtonElement (float duration, System.Action onStart, System.Action onEnd=null) {
		this.duration = duration
		#if FAST_TIME
		* 0.1f
		#endif
		;
		this.onStart = onStart;
		this.onEnd = onEnd;
		Reset ();
	}

	public void StartTimer () {

		if (onStart != null)
			onStart ();

		Interactable = false;
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
		Interactable = true;
		OnUpdateTime (0);
	}

	void OnUpdateTime (float p) {
		float time = Mathf.Round (Mathf.Abs (p * duration - duration));
		Text = time.ToString () + " seconds";
	}
}
