using UnityEngine;
using System.Collections;

namespace Views {

	// Shows the question that players will be debating in this round
	// The Decider is instructed to read a script

	public class Question : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("think_instructions"));
		}

		protected override void OnInitPlayerElements () {
			Elements.Add ("trophy", new ImageElement ("trophy"));
			Elements.Add ("separator", new ImageElement (""));
		}

		protected override void OnInitElements () {
			Elements.Add ("question", new TextElement (Game.Controller.Question));
			Elements.Add ("round", new TextElement ("Round " + (Game.Controller.RoundNumber+1).ToString ()));
		}
	}
}