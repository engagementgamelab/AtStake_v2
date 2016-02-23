using UnityEngine;
using System.Collections;

public class CoinsElement : ScreenElement {

	/*protected override void OnRender (CoinsElementUI c) {
		SetText (Game.Score.PlayerScore);
		Game.Score.onUpdateScore += SetText;
	}

	protected override void OnRemove (CoinsElementUI c) {
		Game.Score.onUpdateScore -= SetText;
	}
*/
	void SetText (int score) {
		// uiElement.Text.text = "Coins: " + score;
	}
}