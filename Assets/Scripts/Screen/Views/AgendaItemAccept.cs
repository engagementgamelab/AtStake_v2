using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	public class AgendaItemAccept : AgendaItemResult {

		protected override void OnInitPlayerElements () {
			if (MyItem) {
				Elements.Add ("agenda_item", new TextElement (
					DataManager.GetTextFromScreen (Model, "your_item", RewardTextVariables)));
			}
		}

		protected override void OnInitElements () {
			if (!MyItem) {
				Elements.Add ("agenda_item", new TextElement (
					DataManager.GetTextFromScreen (Model, "their_item", RewardTextVariables)));
			}
		}

		protected override void OnShow () {
			Game.Score.ApplyPlayerReward (Item);
		}
	}
}