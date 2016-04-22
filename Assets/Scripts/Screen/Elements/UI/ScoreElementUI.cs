using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreElementUI<T> : ScreenElementUI<T> where T : ScoreElement {

	public Text scoreText;
	int score = -1;
	int targetScore = -1;
	float timer = 0f;
	float rate = 0.005f;

	public override void ApplyElement (T e) {
		if (score == -1)
			score = e.Score;
		targetScore = e.Score;
	}

	protected override void OnUpdate (T e) {
		ApplyElement (e);
	}

	protected override void OnSetActive (bool active) {
		Co.WaitForFixedUpdate (() => { // eh, not proud of this hack
			scoreText.ApplyStyle (Style);
		});
	}

	void Update () {
		if (timer < rate) {
			timer += Time.deltaTime;
		} else {
			if (score < targetScore)
				score ++;
			else if (score > targetScore)
				score --;
			scoreText.text = score.ToString ();
			timer = 0;
		}
	}
}
