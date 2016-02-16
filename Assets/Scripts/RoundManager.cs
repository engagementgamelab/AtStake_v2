using UnityEngine;
using System.Collections;

//// <summary>
/// Handles rounds. The number of rounds is determined by the number of questions in the deck.
/// </summary>
public class RoundManager : GameInstanceBehaviour {

	int Count {
		get { return DataManager.GetQuestions (Game.Decks.Name).Length; }
	}

	public int Current { get; private set; }

	public void Init () {
		Current = 0;
		Game.Dispatcher.AddListener ("SetRound", SetRound);
	}

	public bool NextRound () {
		if (Current < Count-1) {
			Current ++;
			Game.Dispatcher.ScheduleMessage ("SetRound", Current);
			return true;
		}
		return false;
	}

	void SetRound (NetworkMessage msg) {
		Current = msg.val;
	}
}
