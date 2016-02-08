using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScreenManager : GameInstanceComponent {

	Dictionary<string, GameScreen> screens;
	Dictionary<string, GameScreen> Screens {
		get {
			if (screens == null) {

				screens = new Dictionary<string, GameScreen> () {
					{ "start", new StartScreen () }
				};
			}

			return screens;
		}
	}

	public void Init (Transform canvas) {
		foreach (var screen in Screens) {
			screen.Value.Init (canvas);
		}
		SetScreen ("start");
	}

	void SetScreen (string id) {
		Screens[id].Show ();
	}
}
