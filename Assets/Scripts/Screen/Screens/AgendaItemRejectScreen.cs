using UnityEngine;
using System.Collections;

public class AgendaItemRejectScreen : AgendaItemResultScreen {
	
	protected override void OnInitPlayerElements () {
		if (MyItem) {
			Elements.Add ("your_item", new TextElement (
				DataManager.GetTextFromScreen (Model, "your_item", RewardTextVariables)));
		}
	}

	protected override void OnInitElements () {
		if (!MyItem) {
			Elements.Add ("their_item", new TextElement (
				DataManager.GetTextFromScreen (Model, "their_item", RewardTextVariables)));
		}		
	}
}