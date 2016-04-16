using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InstructionsElementUI : TextElementUI {

	public Text instructionText;

	public override Text Text {
		get { return instructionText; }
	}

	public override void ApplyElement (TextElement e) {
		Text.text = "\"" + e.Text + "\"";
		Text.ApplyStyle (Style);
	}

	protected override void OnUpdate (TextElement e) { ApplyElement (e); }
}
