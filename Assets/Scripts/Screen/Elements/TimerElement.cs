using UnityEngine;

public enum TimerType { Think, Pitch, Listen, Deliberate }

public class TimerElement : ScreenElement {

	string text;
	public string Text {
		get { return text; }
		set {
			text = value;
			SendUpdateMessage ();
		}
	}

	TimerType type = TimerType.Think;
	public TimerType Type {
		get { return type; }
		set {
			type = value;
			SendUpdateMessage ();
		}
	}

	public float Progress { get; private set; }
	public string TimeText { get; private set; }

	public float Duration {
		get { return duration; }
	}

	float duration;
	System.Action onEnd;

	public TimerElement (string text, float duration, TimerType type, System.Action onEnd=null) {
		Text = text;
		this.duration = duration
		#if FAST_TIME
		* 0.1f
		#endif
		;
		this.type = type;
		this.onEnd = onEnd;
		Reset ();
	}

	public void StartTimer (float startTime=0f) {
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
		OnUpdateTime (0);
	}

	public void Skip () {
		OnUpdateTime (1f);
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
