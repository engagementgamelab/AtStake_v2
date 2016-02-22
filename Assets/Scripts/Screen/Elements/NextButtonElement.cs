using UnityEngine;
using System.Collections;

public class NextButtonElement : ButtonElement {

	public NextButtonElement (string nextScreen, System.Action onPress=null) :
		base ("Next", () => {
			if (nextScreen != "")
				Game.Views.AllGoto (nextScreen);
			if (onPress != null)
				onPress ();
		}) {}
}