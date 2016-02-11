using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Player enters their name on this screen

public class NameScreen : GameScreen {

	protected override void OnInitElements () {
		Elements.Add ("text", new InputElement ("Your name"));
		Elements.Add ("button", new ButtonElement ("Enter", () => { GotoScreen ("hostjoin"); }));
		Elements.Add ("back", new BackButtonElement ("start"));		
	}

	protected override void OnHide () {
		// TODO: don't actually allow players to continue w/o entering a name
		string name = GetScreenElement<InputElement> ("text").Text;
		if (name != "")
			Game.Manager.Player.Name = name;
	}
}
