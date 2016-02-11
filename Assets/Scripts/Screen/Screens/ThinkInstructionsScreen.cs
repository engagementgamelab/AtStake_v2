using UnityEngine;
using System.Collections;

public class ThinkInstructionsScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions", new TextElement ("read this out load: 'EVERYONE!!! we're gonna think for like 30 secs now'"));
		Elements.Add ("next", new ButtonElement ("Next", () => { AllGotoScreen ("think"); }));
	}
}