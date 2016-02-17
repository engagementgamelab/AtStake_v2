using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgendaItemResultScreen : GameScreen {
	
	PlayerAgendaItem item;
	protected PlayerAgendaItem Item {
		get { 
			if (item == null)
				item = Game.Decks.CurrentAgendaItem;
			return item;
		}
	}

	protected bool MyItem { get { return Item.Player == Name; } }

	protected int[] RewardValues {
		get { return DataManager.GetSettings ().Rewards; }
	}

	protected Dictionary<string, string> RewardTextVariables {
		get { 
			Dictionary<string, string> v = new Dictionary<string, string> (TextVariables);
			v.Add ("reward", RewardValues[Item.Reward].ToString ());
			v.Add ("rewarded_player", Item.Player);
			return v;
		}
	}

	bool hasNextItem;

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new NextButtonElement ("", Advance));
		Co.WaitForFixedUpdate (() => {
			hasNextItem = Game.Decks.NextAgendaItem ();
		});
	}

	protected override void OnHide () {
		item = null;
	}

	void Advance () {
		if (hasNextItem) {
			AllGotoScreen ("agenda_item");
		} else {
			AllGotoScreen (Game.Rounds.NextRound ()
				? "scoreboard"
				: "final_scoreboard"
			);
		}
	}
}