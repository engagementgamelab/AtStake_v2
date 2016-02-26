using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PotElementUI : ScreenElementUI<PotElement> {

	public Text potText;

	public override void ApplyElement (PotElement e) {
		potText.text = e.Text;
	}

	protected override void OnUpdate (PotElement e) {
		potText.text = e.Text;
	}
}
