using UnityEngine;
using System.Collections;

// After their pitch time has expired, players are given the opportunity to buy more time

public class ExtraTimeScreen : GameScreen {

	protected override void OnInitElements () {
		Models.Settings settings = DataManager.GetSettings ();
		Elements.Add ("instructions", new TextElement ("Times up but you can pay " + settings.ExtraTimeCost + " for " + settings.ExtraSeconds + " seconds additional time."));
		Elements.Add ("accept", new ButtonElement ("More time", () => {
			GotoScreen ("pitch");
			Game.Dispatcher.ScheduleMessage ("AcceptExtraTime");
		}));
		Elements.Add ("decline", new ButtonElement ("Nah", () => {
			GotoScreen ("pitch");
			Game.Dispatcher.ScheduleMessage ("DeclineExtraTime");	
		}));
	}
}