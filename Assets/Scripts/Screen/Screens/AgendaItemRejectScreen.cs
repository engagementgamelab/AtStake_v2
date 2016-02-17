using UnityEngine;
using System.Collections;

public class AgendaItemRejectScreen : AgendaItemResultScreen {
	
	protected override void OnInitPlayerElements () {
		if (MyItem) {
			// Elements.Add ("your_item", new TextElement ("Decider did not choose your agenda item"));
			Elements.Add ("your_item", new TextElement (Model.Text["your_item"]));
		}
	}

	protected override void OnInitElements () {
		if (!MyItem) {
			// Elements.Add ("their_item", new TextElement ("The agenda item was not approved"));
			Elements.Add ("their_item", new TextElement (Model.Text["their_item"]));
		}		
	}
}