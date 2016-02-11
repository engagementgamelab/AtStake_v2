using UnityEngine;
using System.Collections;

public class GameInstanceBehaviour : MB {

	public GameInstance Game {
		get { return Parent.GetComponent<GameInstance> (); }
	}

	protected void Log (object msg) {
		Debug.Log (Game.Manager.Player.Name + ": " + msg);
	}
}
