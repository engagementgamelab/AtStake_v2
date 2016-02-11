using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInstance : MonoBehaviour {

	public GameInstanceUI Ui { get; private set; }
	public PlayerManager Manager { get; private set; }
	public MultiplayerManager Multiplayer { get; private set; }
	public MessageDispatcher Dispatcher { get; private set; }
	public GameScreenManager Screens { get; private set; }
	public DeckManager Decks { get; private set; }
	bool focused = false;

	public string Name {
		get { return Manager.Player.Name; }
	}

	void Awake () {

		Manager = ObjectPool.Instantiate<PlayerManager> ();
		Manager.transform.SetParent (transform);

		Multiplayer = ObjectPool.Instantiate<MultiplayerManager> ();
		Multiplayer.transform.SetParent (transform);

		Dispatcher = ObjectPool.Instantiate<MessageDispatcher> ();
		Dispatcher.transform.SetParent (transform);

		Transform grid = GameObject.FindWithTag ("LocalDevGrid").transform;
		Ui = ObjectPool.Instantiate<GameInstanceUI> ();
		Ui.transform.SetParent (grid);
		Ui.Init (this);
		
		Screens = ObjectPool.Instantiate<GameScreenManager> ();
		Screens.transform.SetParent (transform);
		Screens.Init (Ui.Transform);

		Decks = ObjectPool.Instantiate<DeckManager> ();
		Decks.transform.SetParent (transform);
		Decks.Init ();
	}

	public void Focus () {
		Ui.Focus ();
		focused = true;
	}

	public void Unfocus () {
		Ui.Unfocus ();
		focused = false;
	}

	public void AddLine (string line) {
		Ui.AddTextLine (line);
	}

	// Could move everything below here to a multiplayer manager
	public void HostGame () {
		Manager.Init ();
		Multiplayer.HostGame ();
		Multiplayer.onUpdateClients += OnUpdateClients;
		Multiplayer.onDisconnect += OnDisconnect;
	}

	public void JoinGame (string hostId="") {
		Manager.Init ();
		Multiplayer.JoinGame (hostId == "" ? Multiplayer.Hosts[0] : hostId);
		Multiplayer.onDisconnect += OnDisconnect;
	}

	void OnUpdateClients (List<string> clients) {
		string players = "";
		foreach (string player in clients) {
			players += player + "|";
		}
		players += Manager.Player.Name;
		Dispatcher.ScheduleMessage ("UpdatePlayers", players);
	}

	void OnDisconnect () {
		Screens.OnDisconnect ();
		Multiplayer.onDisconnect -= OnDisconnect;
		Dispatcher.RemoveAllListeners ();
	}
}
