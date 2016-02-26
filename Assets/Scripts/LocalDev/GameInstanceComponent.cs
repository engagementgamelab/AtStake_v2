using UnityEngine;
using System.Collections;

public class GameInstanceComponent {

	public GameInstanceBehaviour Behaviour { get; private set; }

	GameInstance game;
	public GameInstance Game {
		get {
			if (game == null)
				game = Behaviour.Game;
			return game;
		}
	}

	public void Init (GameInstanceBehaviour behaviour) {
		Behaviour = behaviour;
		OnInit ();
	}

	protected virtual void OnInit () {}

	protected void Log (object msg) {
		Debug.Log (Game.Manager.Player.Name + ": " + msg);
	}
}
