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

	#if UNITY_EDITOR && SINGLE_SCREEN
	string[] names = new [] { "Forrest", "Jenny", "Momma", "Lt Dan" };
	#endif

	void Awake () {
		Application.targetFrameRate = 60;
		Application.runInBackground = true;

		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void AddPlayer () {

		if (instances.Count == 4)
			return;

		GameInstance i = ObjectPool.Instantiate<GameInstance> ();
		i.transform.SetParent (transform);
		instances.Add (i);
		#if UNITY_EDITOR && SINGLE_SCREEN
		i.Manager.Name = names[instances.Count-1];
		#endif
		i.SetTemplatePosition (instancePositions[instances.Count-1]);
	}

	#if UNITY_EDITOR && SINGLE_SCREEN

	Vector3[] instancePositions = new Vector3[] {
		new Vector3 (0, 333, 0),
		new Vector3 (250, 333, 0),
		Vector3.zero,
		new Vector3 (250, 0, 0)
	};

	void Update () {
		if (Input.GetKeyDown (KeyCode.Minus)) {
			AddPlayer ();
		}
	}

	void GotoView (string id) {
		
		// Skip ahead to the view with the given id
		// This must be called *before* any players have been added - it "fakes" a game playthrough
		// (only for testing purposes)

		bool beforeLobby = id == "start" || id == "hostjoin";
		bool beforeDeck = beforeLobby || id == "lobby" || id == "games";
		bool beforeBio = beforeDeck || id == "roles" || id == "pot";
		bool beforeWinner = beforeBio || id == "agenda" || id == "question" || id == "think_instructions" || id == "think" || id == "pitch_instructions" || id == "pitch" || id == "extra_time" || id == "deliberate_instructions" || id == "deliberate" || id == "extra_time_deliberate" || id == "decide";
		bool deck = id == "deck";

		// Create host
		AddPlayer ();

		if (beforeLobby) {
			instances[0].Views.Goto (id);
			for (int i = 1; i < 3; i ++) {
				AddPlayer ();
				instances[i].Views.Goto (id);
			}
			return;
		}

		instances[0].StartGame ();
		instances[0].Multiplayer.HostGame ((ResponseType res) => {

			if (res != ResponseType.Success) {
				Debug.LogWarning ("Failed to run test because the host name is already being used by another open room.");
				return;
			}

			// Move the host to the view
			if (deck) {
				instances[0].Views.Goto ("deck");
			} else if (beforeDeck) {
				instances[0].Views.Goto (id);
			}

			string hostName = instances[0].Name;
			string roomId = instances[0].Multiplayer.RoomId;

			// Create the other players
			for (int i = 1; i < 3; i ++) {

				AddPlayer ();
				GameInstance gi = instances[i];

				gi.Multiplayer.JoinGame (hostName, roomId, (ResponseType response) => {
					gi.StartGame ();
				});
			}
		});


		// WARNING: js-style callback hell approaching
		// Wait for clients to connect
		Co.YieldWhileTrue (() => { return !ClientsConnected (); }, () => {

			if (deck) {
				instances[0].Views.AllGoto ("deck");
			} else if (!beforeDeck) {

				instances[0].Dispatcher.AddListener ("SetDeck", (NetMessage msg) => {
					Co.WaitForFixedUpdate (() => {

						// Once the deck has been set, start the game
						instances[0].Dispatcher.ScheduleMessage ("StartGame");

						// Once the data has loaded, send all players to the supplied view
						Co.YieldWhileTrue (() => { return instances.Find (x => !x.Controller.DataLoaded) != null; }, () => {

							// If the view happens at or after the winner screen, choose a winner
							if (!beforeWinner) {
								string winner = System.Array.Find (instances[0].Controller.Roles, x => x.Title != "Decider").PlayerName;
								for (int i = 0; i < instances.Count; i ++) {
									instances[i].Controller.SetWinner (winner);
								}
							}

							instances[0].Views.AllGoto (id);

							// If the view comes after the pot screen, set the scores
							if (!beforeBio) {
								for (int i = 0; i < instances.Count; i ++) {
									instances[i].Score.FillPot ();
									instances[i].Score.AddRoundStartScores ();
								}
							}
						});
					});
				});

				// Set the default deck
				instances[0].Dispatcher.ScheduleMessage ("SetDeck", "Civic");
			}
		});
	}

	bool PlayersOnView (string viewId) {
		return instances.Find (x => x.Views.CurrView != viewId) == null;
	}

	bool ClientsConnected () {
		return instances[0].Multiplayer.Clients.Count == 2;
	}

	bool ValidView (string view) {
		return view == "start"
			|| view == "name_taken"
			|| view == "hostjoin"
			|| view == "lobby"
			|| view == "games"
			|| view == "deck"
			|| view == "roles"
			|| view == "pot"
			|| view == "bio"
			|| view == "agenda"
			|| view == "question"
			|| view == "think_instructions"
			|| view == "think"
			|| view == "pitch_instructions"
			|| view == "pitch"
			|| view == "extra_time"
			|| view == "deliberate_instructions"
			|| view == "deliberate"
			|| view == "extra_time_deliberate"
			|| view == "decide"
			|| view == "winner"
			|| view == "agenda_item_instructions"
			|| view == "agenda_item"
			|| view == "agenda_item_accept"
			|| view == "agenda_item_reject"
			|| view == "scoreboard"
			|| view == "final_scoreboard"
			|| view == "disconnected"
			|| view == "socket_disconnected";
	}

	string gotoView = "";

	void OnGUI () {
		if (instances.Count > 0 && instances[0].Controller.DataLoaded)
			return;
		GUILayout.BeginHorizontal ();
		if (instances.Count < 4 && GUILayout.Button ("Run test")) {
			GotoView ("lobby");
			Co.YieldWhileTrue (() => { return !ClientsConnected (); }, () => {
				instances[0].Dispatcher.ScheduleMessage ("RunTest");
			});
		}
		if (instances.Count < 4 && GUILayout.Button ("Add player")) {
			AddPlayer ();
		}
		if (instances.Count == 0) {
			GUILayout.Label (" OR: skip to view: ");
			gotoView = GUILayout.TextField (gotoView, 25, GUILayout.Width (50));
			if (gotoView != "" && ValidView (gotoView)) {
				if (GUILayout.Button ("Go")) {
					GotoView (gotoView);
				}
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
