using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Player enters their name on this screen

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
		// TODO: don't actually allow players to continue w/o entering a name
		string name = GetScreenElement<InputElement> ("text").Text;
		if (name != "")
			Game.Manager.Player.Name = name;
	}
}
