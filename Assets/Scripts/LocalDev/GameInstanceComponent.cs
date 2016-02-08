using UnityEngine;
using System.Collections;

public class GameInstanceComponent : MB {

	public GameInstance Game {
		get { return Parent.GetComponent<GameInstance> (); }
	}
}
