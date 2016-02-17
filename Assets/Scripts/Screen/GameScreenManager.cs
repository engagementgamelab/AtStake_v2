using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//// <summary>
/// Manages all the screens in the game
/// Sets which screen is currently active
/// </summary>
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
					{ "question", new QuestionScreen () },
					{ "think_instructions", new ThinkInstructionsScreen () },
					{ "think", new ThinkScreen () },
					{ "pitch_instructions", new PitchInstructionsScreen () },
					{ "pitch", new PitchScreen () },
					{ "extra_time", new ExtraTimeScreen () },
					{ "deliberate_instructions", new DeliberateInstructionsScreen () },
					{ "deliberate", new DeliberateScreen () },
					{ "decide", new DecideScreen () },
					{ "winner", new WinnerScreen () },
					{ "agenda_item", new AgendaItemScreen () },
					{ "agenda_item_accept", new AgendaItemAcceptScreen () },
					{ "agenda_item_reject", new AgendaItemRejectScreen () },
					{ "scoreboard", new ScoreboardScreen () },
					{ "final_scoreboard", new FinalScoreboardScreen () }
				};
			}

			return screens;
		}
	}

	public string CurrScreen { get; private set; }
	public string PrevScreen { get; private set; }

	public void Init (Transform canvas) {
		foreach (var screen in Screens)
			screen.Value.Init (this, canvas, screen.Key);
		SetScreen ("start");
		Game.Dispatcher.AddListener ("GotoScreen", OnGotoScreen);
	}

	public void SetScreen (string id) {
		if (!string.IsNullOrEmpty (CurrScreen)) {
			Screens[CurrScreen].Hide ();
			PrevScreen = CurrScreen;
		}
		CurrScreen = id;
		try {
			Screens[CurrScreen].Show ();
		} catch (KeyNotFoundException e) {
			throw new System.Exception ("No screen with the id '" + id + "' exists.\n" + e);
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
