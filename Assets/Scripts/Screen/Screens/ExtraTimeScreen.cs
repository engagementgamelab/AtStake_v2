using UnityEngine;
using System.Collections;

// After their pitch time has expired, players are given the opportunity to buy more time

public class ExtraTimeScreen : GameScreen {

	protected override void OnInitElements () {
		Models.Settings settings = DataManager.GetSettings ();

		if (Game.Score.CanAffordExtraTime) {
			// Elements.Add ("can_afford", new TextElement ("Times up but you can pay " + settings.ExtraTimeCost + " for " + settings.ExtraSeconds + " seconds additional time."));
			Elements.Add ("can_afford", new TextElement (Model.Text["can_afford"]));
			Elements.Add ("accept", new ButtonElement (Model.Buttons["accept"], () => {
				GotoScreen ("pitch");
				Game.Dispatcher.ScheduleMessage ("AcceptExtraTime", Name);
			}));
		} else {
			Elements.Add ("cant_afford", new TextElement (Model.Text["cant_afford"]));
			// Elements.Add ("cant_afford", new TextElement ("You can't afford extra time :("));
		}

		Elements.Add ("decline", new ButtonElement (Model.Buttons["decline"], () => {
			GotoScreen ("pitch");
			Game.Dispatcher.ScheduleMessage ("DeclineExtraTime");	
		}));
	}
}