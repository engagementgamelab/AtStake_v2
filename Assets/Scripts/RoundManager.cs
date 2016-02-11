using UnityEngine;
using System.Collections;

public class RoundManager : GameInstanceBehaviour {

	int Count {
		get { return DataManager.GetQuestions (Game.Decks.Name).Length; }
	}

	public int Current { get; private set; }

	public void Init () {
		Current = 0;
	}
}
