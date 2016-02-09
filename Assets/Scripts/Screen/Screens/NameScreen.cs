using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NameScreen : GameScreen {

	Dictionary<string, ScreenElement> elements;
	public override Dictionary<string, ScreenElement> Elements {
		get {
			if (elements == null) {
				elements = new Dictionary<string, ScreenElement> ();
				elements.Add ("text", new InputElement ("Your name"));
				elements.Add ("button", new ButtonElement ("Enter", () => { GotoScreen ("hostjoin"); }));
				elements.Add ("back", new BackButtonElement ("start"));
			}
			return elements;
		}
	}

	protected override void OnHide () {
		game.Player.SetName (GetScreenElement<InputElement> ("text").Text);
	}
}
