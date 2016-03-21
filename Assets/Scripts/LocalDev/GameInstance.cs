using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Views;
using Templates;

public class GameInstance : MonoBehaviour {

	public PlayerManager Manager { get; private set; }
	// public MultiplayerManager Multiplayer { get; private set; }
	public MultiplayerManager2 Multiplayer2 { get; private set; }
	public MessageDispatcher Dispatcher { get; private set; }
	public ViewManager Views { get; private set; }
	public TemplateManager Templates { get; private set; }
	public DeckManager Decks { get; private set; }
	public RoundManager Rounds { get; private set; }
	public ScoreManager Score { get; private set; }

	public string Name {
		get { return Manager.Player.Name; }
	}

	void OnEnable () {

		Manager 	= GameInstanceBehaviour.Init<PlayerManager> (transform);
		// Multiplayer = GameInstanceBehaviour.Init<MultiplayerManager> (transform);
		Multiplayer2 = GameInstanceBehaviour.Init<MultiplayerManager2> (transform);
		Dispatcher 	= GameInstanceBehaviour.Init<MessageDispatcher> (transform);
		Views 		= GameInstanceBehaviour.Init<ViewManager> (transform);
		Templates 	= GameInstanceBehaviour.Init<TemplateManager> (transform);
		Decks 		= GameInstanceBehaviour.Init<DeckManager> (transform);
		Rounds 		= GameInstanceBehaviour.Init<RoundManager> (transform);
		Score 		= GameInstanceBehaviour.Init<ScoreManager> (transform);
		
		InitApp ();
	}

	public void SetTemplatePosition (Vector3 pos) {
		Templates.transform.position = pos;
		Templates.transform.SetParent (transform);
	}
	
	// Called when the app is started
	void InitApp () {
		Views.Init ();
		Decks.Init ();
	}

	// Called when the game begins (considered to be when a player hosts or joins a game)
	void StartGame () {
		Manager.Init ();
		Rounds.Init ();
		Score.Init ();
	}

	public void EndGame () {
		// Multiplayer.Disconnect ();
	}

	public void HostGame () {
		/*StartGame ();
		Multiplayer.HostGame ();
		Multiplayer.onUpdateClients += OnUpdateClients;
		Multiplayer.onDisconnect += OnDisconnect;*/
	}

	public void JoinGame (string hostId="") {
		/*StartGame ();
		Multiplayer.JoinGame (hostId == "" ? Multiplayer.Hosts[0] : hostId);
		Multiplayer.onDisconnect += OnDisconnect;*/
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
		/*Views.OnDisconnect ();
		Multiplayer.onDisconnect -= OnDisconnect;
		Dispatcher.RemoveAllListeners ();*/
	}
}
