using UnityEngine;
using System.Collections;

// Decider sees a script to read out loud to players
// Players see their bio (a description of their role)

public class BioScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new NextButtonElement ("agenda"));
	}

	protected override void OnInitPlayerElements () {
		CreateRoleCard (true, true, false);
	}
}