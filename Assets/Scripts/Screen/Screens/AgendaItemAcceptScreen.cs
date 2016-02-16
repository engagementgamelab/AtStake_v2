using UnityEngine;
using System.Collections;

public class AgendaItemAcceptScreen : AgendaItemResultScreen {

	protected override void OnInitPlayerElements () {
		if (MyItem) {
			Elements.Add ("winner", new TextElement ("Decider chose your agenda item!! :D You get " + Item.Reward + " coins"));
		}
	}

	protected override void OnInitElements () {
		if (!MyItem) {
			Elements.Add ("reject", new TextElement (Item.Player + "'s agenda item was accepted. They win " + Item.Reward + " coins"));
		}
	}
}