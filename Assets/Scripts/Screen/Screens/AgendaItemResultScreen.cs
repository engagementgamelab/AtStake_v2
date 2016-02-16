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

	bool hasNextItem;

	protected override void OnInitDeciderElements () {
		Elements.Add ("next", new ButtonElement ("Next", Advance));
		Co.WaitForFixedUpdate (() => {
			hasNextItem = Game.Decks.NextAgendaItem ();
		});
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