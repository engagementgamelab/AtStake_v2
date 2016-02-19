using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Player enters their name on this screen

public class NameScreen : GameScreen {

	protected override void OnInitElements () {
		Elements.Add ("input", new InputElement ("Your name", (string name) => {
			Game.Manager.Player.Name = name;
		}));
		Elements.Add ("submit", new ButtonElement (Model.Buttons["submit"], () => { GotoScreen ("hostjoin"); }));
		Elements.Add ("back", new BackButtonElement ("start"));		
	}

	protected override void OnHide () {
		// TODO: don't actually allow players to continue w/o entering a name
		/*string name = GetScreenElement<InputElement> ("input").Text;
		if (name != "")
			Game.Manager.Player.Name = name;*/
	}
}
