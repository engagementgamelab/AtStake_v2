using UnityEngine;
using System.Collections;

public class AgendaItemAcceptScreen : AgendaItemResultScreen {

	protected override void OnInitPlayerElements () {
		if (MyItem) {
			// Elements.Add ("your_item", new TextElement ("Decider chose your agenda item!! :D You get " + RewardValues[Item.Reward] + " coins"));
			Elements.Add ("your_item", new TextElement (Model.Text["your_item"]));
		}
	}

	protected override void OnInitElements () {
		if (!MyItem) {
			// Elements.Add ("their_item", new TextElement (Item.Player + "'s agenda item was accepted. They win " + RewardValues[Item.Reward] + " coins"));
			Elements.Add ("their_item", new TextElement (Model.Text["their_item"]));
		}
	}

	protected override void OnShow () {
		Game.Score.ApplyPlayerReward (Item);
	}
}