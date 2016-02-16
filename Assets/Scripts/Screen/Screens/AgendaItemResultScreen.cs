using UnityEngine;
using System.Collections;

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

	bool hasNextItem;

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new NextButtonElement ("", Advance));
		Co.WaitForFixedUpdate (() => {
			hasNextItem = Game.Decks.NextAgendaItem ();
		});
	}

	protected override void OnInitElements () {
		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
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