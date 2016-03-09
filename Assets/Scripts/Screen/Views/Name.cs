using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Player enters their name on this screen

	public class Name : View {

		protected override void OnInitElements () {

			Elements.Add ("input", new InputElement ("Your name", (string name) => {
				#if !SINGLE_SCREEN
				GetScreenElement<ButtonElement> ("submit").Interactable = name != "";
				#endif
			}, (string name) => {
				Game.Manager.Player.Name = name;

				// This allows the name to be submitted by pressing "done" on the ios/android keyboard
				if (name != "")
					GotoView ("hostjoin");
			}) {
				FocusedOnLoad = true	
			});

			Elements.Add ("submit", new ButtonElement (Model.Buttons["submit"], () => { 
				GotoView ("hostjoin");
			}) { 
				#if !SINGLE_SCREEN
				Interactable = false 
				#endif
			});

			Elements.Add ("back", new BackButtonElement ("start"));		
		}
	}
}
