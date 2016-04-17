using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinsElementUI : ScreenElementUI<CoinsElement> {

	public Text coinsText;

	public override void ApplyElement (CoinsElement e) {
		coinsText.text = e.Text;
	}

	protected override void OnUpdate (CoinsElement e) {
		ApplyElement (e);
	}

	protected override void OnSetActive (bool active) {
		Co.WaitForFixedUpdate (() => { // eh, not proud of this hack
			coinsText.ApplyStyle (Style);
		});
	}
}