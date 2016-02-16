using UnityEngine;
using System.Collections;

// Decider sees a script to read out loud to players
// Players see their bio (a description of their role)

public class BioScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions1", new TextElement ("Read this out loud: 'Come on yall say what you are!!'"));
		Elements.Add ("instructions2", new TextElement ("When everyone has introduced themselves, press next"));
		Elements.Add ("next", new ButtonElement ("Next", () => { AllGotoScreen ("agenda"); }));
	}

	protected override void OnInitPlayerElements () {
		CreateRoleCard (true, true, false);
	}
}