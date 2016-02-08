using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartScreen : GameScreen {

	List<ScreenElement> elements;
	public override List<ScreenElement> Elements {
		get {
			if (elements == null) {
				elements = new List<ScreenElement> ();
				elements.Add (new TextElement ("@Stake"));
			}
			return elements;
		}
	}
}
