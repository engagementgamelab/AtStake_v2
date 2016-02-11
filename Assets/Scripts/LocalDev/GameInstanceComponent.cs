using UnityEngine;
using System.Collections;

public class GameInstanceComponent {

	public GameInstanceBehaviour Component { get; private set; }

	GameInstance game;
	public GameInstance Game {
		get {
			if (game == null)
				game = Component.Game;
			return game;
		}
	}

	public void Init (GameInstanceBehaviour component) {
		Component = component;
	}

	protected void Log (object msg) {
		Debug.Log (Game.Manager.Player.Name + ": " + msg);
	}
}
