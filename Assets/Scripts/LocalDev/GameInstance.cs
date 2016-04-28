using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Views;
using Templates;

/// <summary>
/// Contains all the managers for an instance of the game. All managers have a reference to GameInstance, and in this way are able to communicate to other managers.
/// </summary>
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

	/// <summary>
	/// Gets the player's name. This is for convenience, as the name is referenced quite a bit
	/// </summary>
	public string Name {
		get { return Manager.Name; }
	}

	void Awake () {

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

	/// <summary>
	/// Sets the position of the template on the screen. In production, this is always [0, 0], but when the SINGLE_SCREEN flag is enabled it positions multiple game instances in a grid.
	/// </summary>
	/// <param name="pos">The position to move the template to</param>
	public void SetTemplatePosition (Vector3 pos) {
		Templates.transform.position = pos;
		Templates.transform.SetParent (transform);
	}
	
	// Called when the app is started
	void InitApp () {
		Audio.Init ();
		Views.Init ();
		Manager.Init ();
		Multiplayer.Init ();
	}

	/// <summary>
	/// Called when the game begins (considered to be when a player hosts or joins a game)
	/// </summary>
	public void StartGame () {
		Views.Init ();
		Manager.Init ();
		Decks.Init ();
		Score.Init ();
		Controller.Init ();
		Test.Init ();
		Multiplayer.Init ();
		Multiplayer.onDisconnected += OnDisconnect;
		Multiplayer.onUpdateDroppedClients += OnUpdateDroppedClients;
	}

	/// <summary>
	/// Called when the game ends (considered to be after the FinalScoreboard screen)
	/// </summary>
	public void EndGame () {
		Multiplayer.Disconnect (); // Also triggers OnDisconnect
		Controller.DeleteData ();
	}

	// Handles intentional and unintentional disconnects from the server
	void OnDisconnect () {
		Views.OnDisconnect ();
		Multiplayer.onDisconnected -= OnDisconnect;
		Multiplayer.onUpdateDroppedClients -= OnUpdateDroppedClients;
		Dispatcher.Reset ();
		Controller.Reset ();
		Decks.Reset ();
		Manager.Reset ();
	}

	// Handles dropped clients (other than this one)
	void OnUpdateDroppedClients (bool hasDroppedClients) {
		Views.OnUpdateDroppedClients (hasDroppedClients);
	}
}
