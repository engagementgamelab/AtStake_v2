using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScreenManager : GameInstanceBehaviour {

	Dictionary<string, GameScreen> screens;
	Dictionary<string, GameScreen> Screens {
		get {
			if (screens == null) {

				screens = new Dictionary<string, GameScreen> () {
					{ "start", new StartScreen () },
					{ "name", new NameScreen () },
					{ "hostjoin", new HostJoinScreen () },
					{ "lobby", new LobbyScreen () },
					{ "games", new GamesScreen () },
					{ "deck", new DeckScreen () },
					{ "roles", new RolesScreen () },
					{ "pot", new PotScreen () },
					{ "bio", new BioScreen () },
					{ "agenda", new AgendaScreen () },
					{ "question", new QuestionScreen () }
				};
			}

			return screens;
		}
	}

	public string CurrScreen { get; private set; }
	public string PrevScreen { get; private set; }

	public void Init (Transform canvas) {
		foreach (var screen in Screens)
			screen.Value.Init (this, canvas);
		SetScreen ("start");
		Game.Dispatcher.AddListener ("GotoScreen", OnGotoScreen);
	}

	public void SetScreen (string id) {
		if (!string.IsNullOrEmpty (CurrScreen)) {
			Screens[CurrScreen].Hide ();
			PrevScreen = CurrScreen;
		}
		try {
			CurrScreen = id;
			Screens[CurrScreen].Show ();
		} catch {
			throw new System.Exception ("No screen with the id '" + id + "' exists");
		}
	}

	public void SetScreenPrevious () {
		SetScreen (PrevScreen);
	}

	void OnGotoScreen (NetworkMessage msg) {
		SetScreen (msg.str1);
	}

	public void OnDisconnect () {
		Screens[CurrScreen].OnDisconnect ();
	}
}
