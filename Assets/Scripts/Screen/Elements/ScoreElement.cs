using UnityEngine;
using System.Collections;

public class ScoreElement : ScreenElement {

	string text;
	public string Text {
		get { return text; }
		set {
			text = value;
			SendUpdateMessage ();
		}
	}

	int score;
	public int Score {
		get { return score; }
		set {
			score = value;
			SendUpdateMessage ();
		}
	}

	protected override void OnInit () {
		Game.Score.onUpdateScore += SetText;
		SetText (Game.Score.PlayerScore);
	}

	protected void SetText (int score) {
		Text = score.ToString ();
		Score = score;
	}
}
