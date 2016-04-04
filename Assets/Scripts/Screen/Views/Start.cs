using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// The initial screen on loading the game

	public class Start : View {

		protected override void OnInitElements () {
			
			Elements.Add ("logo", new ImageElement ("logo"));
			Elements.Add ("input", new InputElement ("your name", (string name) => {
				#if !SINGLE_SCREEN
				GetScreenElement<ButtonElement> ("submit").Interactable = name != "";
				#endif
			}, (string name) => {
				Game.Manager.Name = name;
				
				// This allows the name to be submitted by pressing "done" on the ios/android keyboard
				if (name != "")
					GotoView ("hostjoin");
			}));

			Elements.Add ("submit", new ButtonElement (GetButton ("submit"), () => { 
				if (Game.Manager.Name != "")
					GotoView ("hostjoin");
			}) {
				#if !SINGLE_SCREEN
				Interactable = false 
				#endif
			});
		}
	}
}