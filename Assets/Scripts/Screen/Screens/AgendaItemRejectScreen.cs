using UnityEngine;
using System.Collections;

public class AgendaItemRejectScreen : AgendaItemResultScreen {
	
	protected override void OnInitPlayerElements () {
		if (MyItem) {
			Elements.Add ("loser", new TextElement ("Decider did not choose your agenda item"));
		}
	}

	protected override void OnInitElements () {
		if (!MyItem) {
			Elements.Add ("reject", new TextElement ("The agenda item was not approved"));
		}		
	}
}