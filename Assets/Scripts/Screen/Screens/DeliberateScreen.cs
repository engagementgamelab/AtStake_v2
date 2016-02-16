using UnityEngine;
using System.Collections;

// Players and Decider deliberate over who has the best plan
// Players try to get the winning plan to include their agenda items

public class DeliberateScreen : GameScreen {

	float Duration {
		get { return DataManager.GetSettings ().DeliberateSeconds; }
	}

	protected override void OnInitDeciderElements () {
		Elements.Add ("instructions", new TextElement ("When everyone is ready, start the timer"));
		Elements.Add ("timer", new TimerButtonElement (Duration, () => {
			Game.Dispatcher.ScheduleMessage ("StartTimer");
		}, () => {
			AllGotoScreen ("decide");
		}));
	}

	protected override void OnInitElements () {
		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
	}

	protected override void OnInitPlayerElements () {
		CreateRoleCard (true, true, true);
		Elements.Add ("timer", new TimerElement (Duration));
	}

	protected override void OnShow () {
		Game.Dispatcher.AddListener ("StartTimer", StartTimer);
	}

	protected override void OnHide () {
		Game.Dispatcher.RemoveListener (StartTimer);
	}

	void StartTimer (NetworkMessage msg) {
		if (!IsDecider) {
			GetScreenElement<TimerElement> ("timer").StartTimer ();
		}
	}
}
