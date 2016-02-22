using UnityEngine;
using System.Collections;

namespace Views {

	// Displays instructions for the Decider to read out loud

	public class PitchInstructions : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("pitch"));
		}
	}
}