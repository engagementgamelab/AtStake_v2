using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartScreen : GameScreen {

	Dictionary<string, ScreenElement> elements;
	public override Dictionary<string, ScreenElement> Elements {
		get {
			if (elements == null) {
				elements = new Dictionary<string, ScreenElement> ();
				elements.Add ("text", new TextElement ("@Stake"));
				elements.Add ("button", new ButtonElement ("Play", () => { GotoScreen ("name"); }));
			}
			return elements;
		}
	}
}
