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

		if (instances.Count == 4)
			return;

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
			GotoView ("think");
		}

		if (Input.GetKeyDown (KeyCode.Minus)) {
			// add game instance
			AddPlayer ();
		}
	}

	void GotoView (string id) {
		
		// Skip ahead to the view with the given id
		// This must be called *before* any players have been added - it "fakes" a game playthrough
		// (only for testing purposes)

		if (id == "lobby")
			throw new System.Exception ("Cannot skip to lobby because player must be specified as host or client");

		bool beforeLobby = id == "start" || id == "hostjoin";
		bool beforeDeck = beforeLobby || id == "lobby" || id == "games";
		bool beforeBio = beforeDeck || id == "roles" && id == "pot";
		bool beforeWinner = beforeBio || id == "agenda" || id == "question" || id == "think_instructions" || id == "think" || id == "pitch_instructions" || id == "pitch" || id == "extra_time" || id == "deliberate_instructions" || id == "deliberate" || id == "extra_time_deliberate" || id == "decide";
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
						Co.YieldWhileTrue (() => { return instances.Find (x => !x.Controller.DataLoaded) != null;/*!instances[0].Controller.DataLoaded*/; }, () => {

							instances[0].Views.AllGoto (id);

							// If the view comes after the pot screen, set the scores
							if (!beforeBio) {
								for (int i = 0; i < instances.Count; i ++) {
									instances[i].Score.FillPot ();
									instances[i].Score.AddRoundStartScores ();
								}
							}

							// If the view happens at or after the winner screen, choose a winner
							if (!beforeWinner) {
								string winner = System.Array.Find (instances[0].Controller.Roles, x => x.Title != "Decider").PlayerName;
								for (int i = 0; i < instances.Count; i ++) {
									instances[i].Controller.SetWinner (winner);
								}
							}
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

	string gotoView = "";

	void OnGUI () {
		GUILayout.BeginHorizontal ();
		if (instances.Count < 4 && GUILayout.Button ("Add player")) {
			AddPlayer ();
		}
		if (instances.Count == 0) {
			GUILayout.Label (" OR: skip to view: ");
			gotoView = GUILayout.TextField (gotoView, 25, GUILayout.Width (50));
			if (gotoView != "" && GUILayout.Button ("Go")) {
				GotoView (gotoView);
			}
		}
		GUILayout.EndHorizontal ();
	}

	#else

	Vector3[] instancePositions = new Vector3[] { Vector3.zero };

	void Start () {
		AddPlayer ();
	}

	#endif
}
