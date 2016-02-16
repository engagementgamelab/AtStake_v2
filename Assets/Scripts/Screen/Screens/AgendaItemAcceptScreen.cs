using UnityEngine;
using System.Collections;

public class AgendaItemAcceptScreen : AgendaItemResultScreen {

	protected override void OnInitPlayerElements () {
		if (MyItem) {
			Elements.Add ("winner", new TextElement ("Decider chose your agenda item!! :D You get " + RewardValues[Item.Reward] + " coins"));
		}
	}

	protected override void OnInitElements () {
		base.OnInitElements ();
		if (!MyItem) {
			Elements.Add ("reject", new TextElement (Item.Player + "'s agenda item was accepted. They win " + RewardValues[Item.Reward] + " coins"));
		}
	}

	protected override void OnShow () {
		Game.Score.ApplyPlayerReward (Item);
	}
}