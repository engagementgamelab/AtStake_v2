using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Player enters their name on this screen

	public class Name : View {

		protected override void OnInitElements () {
			Elements.Add ("input", new InputElement ("Your name", (string name) => {
				Game.Manager.Player.Name = name;
			}));
			Elements.Add ("submit", new ButtonElement (Model.Buttons["submit"], () => { GotoView ("hostjoin"); }));
			Elements.Add ("back", new BackButtonElement ("start"));		
		}
	}
}