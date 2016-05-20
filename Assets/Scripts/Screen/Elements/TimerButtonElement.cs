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

	public float Progress { get; set; }
	public string TimeText { get; private set; }

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
		set { duration = value; }
	}

	public float Remaining {
		get { return Mathf.Abs (Progress * Duration - Duration); }
	}

	float duration;
	System.Action onStart;
	System.Action onEnd;

	public TimerButtonElement (string text, float duration, System.Action onStart, System.Action onEnd=null) {
		Text = text;
		this.duration = duration
		#if FAST_TIME
		* 0.1f
		#endif
		;
		this.onStart = onStart;
		this.onEnd = onEnd;
		Reset ();
	}

	public void StartTimer (float startTime=0f) {

		if (onStart != null)
			onStart ();

		Interactable = false;
		Co.StartCoroutine (startTime, duration, OnUpdateTime, () => {
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

	public void Skip () {
		OnUpdateTime (1f);
		Interactable = false;
		if (onEnd != null)
			onEnd ();
	}

	void OnUpdateTime (float p) {
		Progress = p;

		float time = Mathf.Round (Mathf.Abs (p * duration - duration));
		TimeText = time.ToString () + " seconds";
		SendUpdateMessage ();
	}
}
