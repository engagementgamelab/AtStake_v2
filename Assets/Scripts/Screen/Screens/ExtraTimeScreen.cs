using UnityEngine;
using System.Collections;

// After their pitch time has expired, players are given the opportunity to buy more time

public class ExtraTimeScreen : GameScreen {

	protected override void OnInitElements () {
		if (Game.Score.CanAffordExtraTime) {
			Elements.Add ("can_afford", new TextElement (DataManager.GetTextFromScreen (Model, "can_afford", TextVariables)));//Model.Text["can_afford"]));
			Elements.Add ("accept", new ButtonElement (Model.Buttons["accept"], () => {
				GotoScreen ("pitch");
				Game.Dispatcher.ScheduleMessage ("AcceptExtraTime", Name);
			}));
		} else {
			Elements.Add ("cant_afford", new TextElement (DataManager.GetTextFromScreen (Model, "can_afford", TextVariables)));//Model.Text["cant_afford"]));
		}

		Elements.Add ("decline", new ButtonElement (Model.Buttons["decline"], () => {
			GotoScreen ("pitch");
			Game.Dispatcher.ScheduleMessage ("DeclineExtraTime");	
		}));
	}
}