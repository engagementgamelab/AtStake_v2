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
	string[] names = new [] { "Forrest", "Jenny", "Momma", "Lt Dan" };

	void Awake () {
		Application.targetFrameRate = 60;
		Application.runInBackground = true;
	}

	void AddPlayer () {
		GameInstance i = ObjectPool.Instantiate<GameInstance> ();
		i.transform.SetParent (transform);
		instances.Add (i);
		i.Manager.Player.Name = names[instances.Count-1];
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
			// Skip to roles screen
			SetupRoles ();
		}

		if (Input.GetKeyDown (KeyCode.RightBracket)) {
			// Skip to decide screen (end of round 1)
			SetupRoles (() => {
				Co.YieldWhileTrue (() => { return instances.Find (x => x.Views.CurrView != "roles"); }, () => {
					instances[0].Dispatcher.ScheduleMessage ("GotoView", "decide");
				});
			});
		}

		if (Input.GetKeyDown (KeyCode.Backslash)) {
			// Skip to last round
			SetupRoles ();
			GameInstance i = instances[0];
			i.Rounds.NextRound ();
			i.Dispatcher.ScheduleMessage ("ChooseWinner", i.Manager.Player.Name);
			i.Rounds.NextRound ();
			i.Dispatcher.ScheduleMessage ("ChooseWinner", instances[1].Manager.Player.Name);
			i.Dispatcher.ScheduleMessage ("GotoView", "roles");
		}

		if (Input.GetKeyDown (KeyCode.Equals)) {
			// Skip to decks screen
			SetupGame ();
		}

		if (Input.GetKeyDown (KeyCode.Minus)) {
			// add game instance
			AddPlayer ();
		}
	}

	void SetupRoles (System.Action callback=null) {
		SetupGame ();
		Co.YieldWhileTrue (() => { return MyNetworkDiscovery.broadcasting == null || instances.Find (x => !x.Multiplayer.Connected) != null; }, () => {
			instances[0].Dispatcher.ScheduleMessage ("SetDeck", "Default");
			instances[0].Dispatcher.ScheduleMessage ("GotoView", "roles");
			if (callback != null)
				callback ();
		});
	}

	void SetupGame () {
		AddPlayer ();
		instances[0].HostGame ();
		Co.YieldWhileTrue (() => { return MyNetworkDiscovery.broadcasting == null; }, () => {
			instances[0].Views.Goto ("deck");
			for (int i = 1; i < 3; i ++) {
				AddPlayer ();
				GameInstance gi = instances[i];
				gi.Multiplayer.RequestHostList((List<string> hosts) => {
					gi.JoinGame ();
					gi.Views.Goto ("deck");
				});
			}
		});
	}

	#else

	Vector3[] instancePositions = new Vector3[] { Vector3.zero };

	void Start () {
		AddPlayer ();
	}
	
	#endif
}
