using UnityEngine;
using System.Collections;

public class AgendaItemScreen : GameScreen {

	PlayerAgendaItem Item {
		get { return Game.Decks.CurrentAgendaItem; }
	}

	protected override void OnInitDeciderElements () {
		Elements.Add ("accept", new ButtonElement (Model.Buttons["accept"], () => { AllGotoScreen ("agenda_item_accept"); }));
		Elements.Add ("reject", new ButtonElement (Model.Buttons["reject"], () => { AllGotoScreen ("agenda_item_reject"); }));
	}

	protected override void OnInitPlayerElements () {
		if (Item.Player == Name)
			Elements.Add ("your_item", new TextElement (Model.Text["your_item"]));
	}

	protected override void OnInitElements () {
		Elements.Add ("item", new TextElement (Item.Description));
	}
}