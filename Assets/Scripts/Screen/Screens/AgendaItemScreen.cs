using UnityEngine;
using System.Collections;

public class AgendaItemScreen : GameScreen {

	PlayerAgendaItem Item {
		get { return Game.Decks.CurrentAgendaItem; }
	}

	protected override void OnInitDeciderElements () {
		Elements.Add ("accept", new ButtonElement ("Yes", () => { AllGotoScreen ("agenda_item_accept"); }));
		Elements.Add ("reject", new ButtonElement ("No", () => { AllGotoScreen ("agenda_item_reject"); }));
	}

	protected override void OnInitPlayerElements () {
		if (Item.Player == Name)
			Elements.Add ("yours", new TextElement ("This is your agenda item! Convince the decider you're awesome!!"));
	}

	protected override void OnInitElements () {
		Elements.Add ("instructions", new TextElement ("Did the winning plan include this agenda item?"));
		Elements.Add ("item", new TextElement (Item.Description));
		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
	}
}