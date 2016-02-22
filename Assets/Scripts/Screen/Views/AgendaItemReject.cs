using UnityEngine;
using System.Collections;

namespace Views {

	public class AgendaItemReject : AgendaItemResult {
		
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
}