using UnityEngine;
using System.Collections;

public class ThinkScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions", new TextElement ("When everyone is ready, start the timer"));
		// TODO: add timer element
		Elements.Add ("timer", new ButtonElement ("30 seconds", () => {}));
	}

	protected override void OnInitPlayerElements () {
		// TODO: add timer element
		Elements.Add ("timer", new TextElement ("30 seconds"));
	}
}
