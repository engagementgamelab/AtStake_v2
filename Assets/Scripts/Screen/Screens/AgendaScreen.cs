using UnityEngine;
using System.Collections;

// Decider sees a script to read out loud to players
// Players see their secret agenda

public class AgendaScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new NextButtonElement ("question"));
	}

	protected override void OnInitPlayerElements () {
		CreateRoleCard (true, false, true);
	}
}