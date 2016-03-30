﻿using UnityEngine;
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
		
		InitApp ();
	}

	public void SetTemplatePosition (Vector3 pos) {
		Templates.transform.position = pos;
		Templates.transform.SetParent (transform);
	}
	
	// Called when the app is started
	void InitApp () {
		Manager.Init ();
		Views.Init ();
		Decks.Init ();
	}

	// Called when the game begins (considered to be when a player hosts or joins a game)
	public void StartGame () {
		Score.Init ();
		Controller.Init ();
		Multiplayer.onDisconnected += OnDisconnect;
	}

	public void EndGame () {
		Multiplayer.Disconnect ();
		Controller.Reset ();
		Decks.Reset ();
		Manager.Reset ();
	}

	void OnDisconnect () {
		Views.OnDisconnect ();
		Multiplayer.onDisconnected -= OnDisconnect;
		Dispatcher.RemoveAllListeners ();
	}
}
