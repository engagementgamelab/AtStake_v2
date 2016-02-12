using UnityEngine;
using System.Collections;

// Decider sees a script to read out loud to players
// Players see their secret agenda

public class AgendaScreen : GameScreen {

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions1", new TextElement ("Read this out loud: 'everyone secretly review yr agenda'"));
		Elements.Add ("instructions2", new TextElement ("Press next when everyone is ready"));
		Elements.Add ("next", new ButtonElement ("Next", () => { AllGotoScreen ("question"); }));
	}

	protected override void OnInitPlayerElements () {
		CreateRoleCard (true, false, true);
		/*Elements.Add ("title", new TextElement (Name + " the " + Title));
		for (int i = 0; i < Role.AgendaItems.Length; i ++) {
			Elements.Add ("agenda" + i.ToString (), new TextElement (Role.AgendaItems[i].Description));
			Elements.Add ("reward" + i.ToString (), new TextElement ("Reward: " + Role.AgendaItems[i].Reward));
		}*/
	}
}