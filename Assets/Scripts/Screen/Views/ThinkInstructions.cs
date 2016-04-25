﻿using UnityEngine;
using System.Collections;

namespace Views {

	// Displays instructions for the Decider to read out loud

	public class ThinkInstructions : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("think"));
		}

		protected override void OnInitPlayerElements () {
			Elements.Add ("listen", new ImageElement ("listen"));
		}
	}
}