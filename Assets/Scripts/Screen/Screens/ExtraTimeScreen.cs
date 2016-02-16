using UnityEngine;
using System.Collections;

// After their pitch time has expired, players are given the opportunity to buy more time

public class ExtraTimeScreen : GameScreen {

	protected override void OnInitElements () {
		Models.Settings settings = DataManager.GetSettings ();

		if (Game.Score.CanAffordExtraTime) {
			Elements.Add ("instructions", new TextElement ("Times up but you can pay " + settings.ExtraTimeCost + " for " + settings.ExtraSeconds + " seconds additional time."));
			Elements.Add ("accept", new ButtonElement ("More time", () => {
				GotoScreen ("pitch");
				Game.Dispatcher.ScheduleMessage ("AcceptExtraTime", Name);
			}));
		} else {
			Elements.Add ("instructions", new TextElement ("You can't afford extra time :("));
		}

		Elements.Add ("decline", new ButtonElement ("Nah", () => {
			GotoScreen ("pitch");
			Game.Dispatcher.ScheduleMessage ("DeclineExtraTime");	
		}));

		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
	}
}