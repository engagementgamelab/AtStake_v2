using UnityEngine;
using System.Collections;

public class InputElementUI : ScreenElementUI<InputElement> {

	public override void ApplyElement (InputElement e) {
		Placeholder.text = e.placeholder;
	}
}
