using UnityEngine;
using System.Collections;

public class GameInstanceBehaviour : MB {

	public GameInstance Game {
		get { return Parent.GetComponent<GameInstance> (); }
	}

	public static T Init<T> (Transform parent) where T : GameInstanceBehaviour {
		T t = ObjectPool.Instantiate<T> ();
		t.Transform.SetParent (parent);
		return t;
	}

	protected void Log (object msg) {
		Debug.Log (Game.Name + ": " + msg);
	}
}
