using UnityEngine;
using System.Collections;

public class BackButtonElementUI : ScreenElementUI<BackButtonElement> {

	public override void ApplyElement (BackButtonElement e) {
		RemoveButtonListeners ();
		AddButtonListener (e.OnPress);
	}
}
