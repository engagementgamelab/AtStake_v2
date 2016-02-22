using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Views;
using Templates;

public class GameInstance : MonoBehaviour {

	public PlayerManager Manager { get; private set; }
	public MultiplayerManager Multiplayer { get; private set; }
	public MessageDispatcher Dispatcher { get; private set; }

	public ViewManager Views { get; private set; }
	public TemplateManager Templates { get; private set; }

	public DeckManager Decks { get; private set; }
	public RoundManager Rounds { get; private set; }
	public ScoreManager Score { get; private set; }

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

		/*Transform grid = GameObject.FindWithTag ("LocalDevGrid").transform;
		Ui = ObjectPool.Instantiate<GameInstanceUI> ();
		Ui.transform.SetParent (grid);*/
		Templates = ObjectPool.Instantiate<TemplateManager> ();

		Views = ObjectPool.Instantiate<ViewManager> ();
		Views.transform.SetParent (transform);

		Decks = ObjectPool.Instantiate<DeckManager> ();
		Decks.transform.SetParent (transform);

		Rounds = ObjectPool.Instantiate<RoundManager> ();
		Rounds.transform.SetParent (transform);

		Score = ObjectPool.Instantiate<ScoreManager> ();
		Score.transform.SetParent (transform);

		InitApp ();
	}

	public void SetTemplatePosition (Vector3 pos) {
		Templates.transform.position = pos;
	}
	
	// Called when the app is started
	void InitApp () {
		Templates.Init (this);
		Views.Init (Templates.Transform);
		Decks.Init ();
	}

	// Called when the game begins (considered to be when a player hosts or joins a game)
	void StartGame () {
		Manager.Init ();
		Rounds.Init ();
		Score.Init ();
	}

	public void EndGame () {
		Multiplayer.Disconnect ();
	}

	public void HostGame () {
		StartGame ();
		Multiplayer.HostGame ();
		Multiplayer.onUpdateClients += OnUpdateClients;
		Multiplayer.onDisconnect += OnDisconnect;
	}

	public void JoinGame (string hostId="") {
		StartGame ();
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
		Views.OnDisconnect ();
		Multiplayer.onDisconnect -= OnDisconnect;
		Dispatcher.RemoveAllListeners ();
	}
}
