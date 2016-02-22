using UnityEngine;
using System.Collections;

namespace Views {

	// Decider sees a script to read out loud to players
	// Players see their secret agenda

	public class Agenda : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("question"));
		}

		protected override void OnInitPlayerElements () {
			CreateRoleCard (true, false, true);
		}
	}
}