using UnityEngine;
using System.Collections;

public class PotElement : ScoreElement {

	protected override void OnInit () {
		Game.Score.onUpdatePot += SetText;
		SetText (Game.Score.Pot);
	}
}