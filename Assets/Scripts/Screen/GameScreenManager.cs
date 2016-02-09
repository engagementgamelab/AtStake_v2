using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScreenManager : GameInstanceComponent {

	Dictionary<string, GameScreen> screens;
	Dictionary<string, GameScreen> Screens {
		get {
			if (screens == null) {

				screens = new Dictionary<string, GameScreen> () {
					{ "start", new StartScreen () },
					{ "name", new NameScreen () },
					{ "hostjoin", new HostJoinScreen () }
				};
			}

			return screens;
		}
	}

	string currScreen = "";
	string prevScreen = "";

	public void Init (Transform canvas) {
		foreach (var screen in Screens)
			screen.Value.Init (Game, canvas);
		SetScreen ("start");
	}

	public void SetScreen (string id) {
		try {
			if (currScreen != "") {
				Screens[currScreen].Hide ();
				prevScreen = currScreen;
			}
			currScreen = id;
			Screens[currScreen].Show ();
		} catch {
			throw new System.Exception ("No screen with the id '" + id + "' exists");
		}
	}

	// not being used rn - could it be removed?
	public void SetScreenPrevious () {
		SetScreen (prevScreen);
	}
}
