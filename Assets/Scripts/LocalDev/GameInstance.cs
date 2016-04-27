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
	public ScoreManager Score { get; private set; }
	public GameController Controller { get; private set; }
	public AudioController Audio { get; private set; }
	public GameTest Test { get; private set; }

	// For convenience (the player's name gets referenced quite a bit)
	public string Name {
		get { return Manager.Name; }
	}

	void OnEnable () {

		Manager 	= GameInstanceBehaviour.Init<PlayerManager> (transform);
		Multiplayer = GameInstanceBehaviour.Init<MultiplayerManager> (transform);
		Dispatcher 	= GameInstanceBehaviour.Init<MessageDispatcher> (transform);
		Views 		= GameInstanceBehaviour.Init<ViewManager> (transform);
		Templates 	= TemplateManager.Init (transform);
		Decks 		= GameInstanceBehaviour.Init<DeckManager> (transform);
		Score 		= GameInstanceBehaviour.Init<ScoreManager> (transform);
		Controller 	= GameInstanceBehaviour.Init<GameController> (transform);
		Audio 		= GameInstanceBehaviour.Init<AudioController> (transform);
		Test 		= GameInstanceBehaviour.Init<GameTest> (transform);
		
		InitApp ();
	}

	public void SetTemplatePosition (Vector3 pos) {
		Templates.transform.position = pos;
		Templates.transform.SetParent (transform);
	}
	
	// Called when the app is started
	void InitApp () {
		Audio.Init ();
		Views.Init ();
		Manager.Init ();
	}

	// Called when the game begins (considered to be when a player hosts or joins a game)
	public void StartGame () {
		Views.Init ();
		Manager.Init ();
		Decks.Init ();
		Score.Init ();
		Controller.Init ();
		Test.Init ();
		Multiplayer.onDisconnected += OnDisconnect;
		Multiplayer.onClientDropped += OnClientDropped;
	}

	public void EndGame () {
		Multiplayer.Disconnect (); // Also triggers OnDisconnect
		Controller.DeleteData ();
	}

	void OnDisconnect () {
		Views.OnDisconnect ();
		Multiplayer.onDisconnected -= OnDisconnect;
		Dispatcher.Reset ();
		Controller.Reset ();
		Decks.Reset ();
		Manager.Reset ();
	}

	void OnClientDropped () {
		Views.OnClientDropped ();
	}
}
