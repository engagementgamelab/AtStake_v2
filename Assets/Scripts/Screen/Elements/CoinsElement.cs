using UnityEngine;
using System.Collections;

public class CoinsElement : ScoreElement {

	protected override void OnInit () {
		Game.Score.onUpdateScore += SetText;
		SetText (Game.Score.PlayerScore);
		audioClip = "coins";
	}
}