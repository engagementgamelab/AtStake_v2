using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInstanceManager : MonoBehaviour {

	static GameInstanceManager instance = null;
		static public GameInstanceManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (GameInstanceManager)) as GameInstanceManager;
				if (instance == null) {
					GameObject go = new GameObject ("GameInstanceManager");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<GameInstanceManager>();
				}
			}
			return instance;
		}
	}

	List<GameInstance> instances = new List<GameInstance> ();

	#if SINGLE_SCREEN
	string[] names = new [] { "Forrest", "Jenny", "Momma", "Lt Dan" };
	#endif

	void Awake () {
		Application.targetFrameRate = 60;
		Application.runInBackground = true;
	}

	void AddPlayer () {
		GameInstance i = ObjectPool.Instantiate<GameInstance> ();
		i.transform.SetParent (transform);
		instances.Add (i);
		#if SINGLE_SCREEN
		i.Manager.Name = names[instances.Count-1];
		#endif
		i.SetTemplatePosition (instancePositions[instances.Count-1]);
	}

	#if SINGLE_SCREEN

	Vector3[] instancePositions = new Vector3[] {
		new Vector3 (0, 333, 0),
		new Vector3 (250, 333, 0),
		Vector3.zero,
		new Vector3 (250, 0, 0)
	};

	void Update () {
		
		if (Input.GetKeyDown (KeyCode.LeftBracket)) {
		}

		if (Input.GetKeyDown (KeyCode.RightBracket)) {
			GotoView ("pitch");
		}

		if (Input.GetKeyDown (KeyCode.Backslash)) {
			
			GotoView ("scoreboard");

			/*// Skip to last round
			GameInstance i = instances[0];
			i.Rounds.NextRound ();
			i.Dispatcher.ScheduleMessage ("ChooseWinner", i.Manager.Player.Name);
			i.Rounds.NextRound ();
			i.Dispatcher.ScheduleMessage ("ChooseWinner", instances[1].Manager.Player.Name);
			i.Dispatcher.ScheduleMessage ("GotoView", "roles");*/
		}

		if (Input.GetKeyDown (KeyCode.Equals)) {
			// Skip to decks screen
			GotoView ("deck");
		}

		if (Input.GetKeyDown (KeyCode.Minus)) {
			// add game instance
			AddPlayer ();
		}
	}

	void GotoView (string id) {
		
		if (id == "lobby")
			throw new System.Exception ("Cannot skip to lobby because player must be specified as host or client");

		bool beforeLobby = id == "start" || id == "hostjoin";
		bool beforeDeck = id == "start" || id == "hostjoin" || id == "lobby" || id == "games";
		bool deck = id == "deck";

		// Create host
		AddPlayer ();

		if (beforeLobby) {
			instances[0].Views.Goto (id);
			for (int i = 1; i < 3; i ++) {
				AddPlayer ();
				GameInstance gi = instances[i];
				instances[i].Views.Goto (id);
			}
			return;
		}

		instances[0].StartGame ();
		instances[0].Multiplayer.HostGame ();

		Co.YieldWhileTrue (() => { return DiscoveryService.Broadcaster == null; }, () => {

			if (deck) {
				instances[0].Views.Goto ("deck");
			} else if (beforeDeck) {
				instances[0].Views.Goto (id);
			}

			for (int i = 1; i < 3; i ++) {
				AddPlayer ();
				GameInstance gi = instances[i];
				gi.Multiplayer.RequestHostList ((List<string> hosts) => {
					gi.Multiplayer.JoinGame (instances[0].Name, (string response) => {
						gi.StartGame ();
						if (deck) {
							gi.Views.Goto ("deck");
						} else if (beforeDeck) {
							gi.Views.Goto (id);
						}
					});
				});
			}
		});

		// WARNING: js-style callback hell approaching

		if (deck) {

			// Skip to the deck view
			Co.YieldWhileTrue (() => { return !PlayersOnView ("deck"); }, () => {
				instances[0].Dispatcher.ScheduleMessage ("SetDeck", "Default");
			});
		} else if (!beforeDeck) {

			// Wait for clients to connect
			Co.YieldWhileTrue (() => { return instances[0].Multiplayer.Clients.Count < 2; }, () => {

				instances[0].Dispatcher.AddListener ("SetDeck", (MasterMsgTypes.GenericMessage msg) => {
					Co.WaitForFixedUpdate (() => {

						// Once the deck has been set, start the game
						instances[0].Dispatcher.ScheduleMessage ("StartGame");

						// Once the data has loaded, send all players to the supplied view
						Co.YieldWhileTrue (() => { return !instances[0].Controller.DataLoaded; }, () => {
							instances[0].Views.AllGoto (id);
						});
					});
				});

				// Set the default deck
				instances[0].Dispatcher.ScheduleMessage ("SetDeck", "Default");
			});
		}
	}

	bool PlayersOnView (string viewId) {
		return instances.Find (x => x.Views.CurrView != viewId) == null;
	}

	#else

	Vector3[] instancePositions = new Vector3[] { Vector3.zero };

	void Start () {
		AddPlayer ();
	}

	#endif
}
