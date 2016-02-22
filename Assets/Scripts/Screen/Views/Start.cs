using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// The initial screen on loading the game

	public class Start : View {

		protected override void OnInitElements () {
			Elements.Add ("play", new ButtonElement (Model.Buttons["play"], () => { GotoView ("name"); }));	
		}
	}
}