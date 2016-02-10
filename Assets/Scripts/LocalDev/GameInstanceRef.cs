using UnityEngine;
using System.Collections;

public class GameInstanceRef {

	public GameInstanceComponent Component { get; private set; }

	GameInstance game;
	public GameInstance Game {
		get {
			if (game == null)
				game = Component.Game;
			return game;
		}
	}

	public void Init (GameInstanceComponent component) {
		Component = component;
	}
}
