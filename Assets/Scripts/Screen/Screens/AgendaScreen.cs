using UnityEngine;
using System.Collections;

// Decider sees a script to read out loud to players
// Players see their secret agenda

public class AgendaScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions1", new TextElement ("Read this out loud: 'everyone secretly review yr agenda'"));
		Elements.Add ("instructions2", new TextElement ("Press next when everyone is ready"));
		Elements.Add ("next", new NextButtonElement ("question"));
	}

	protected override void OnInitPlayerElements () {
		CreateRoleCard (true, false, true);
	}

	protected override void OnInitElements () {
		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
	}
}