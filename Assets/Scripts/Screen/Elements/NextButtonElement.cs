using UnityEngine;
using System.Collections;

public class NextButtonElement : ButtonElement {

	public NextButtonElement (string nextScreen, System.Action onPress=null) :
		base ("Next", () => {
			if (nextScreen != "")
				screen.AllGotoScreen (nextScreen);
			if (onPress != null)
				onPress ();
		}) {}
}