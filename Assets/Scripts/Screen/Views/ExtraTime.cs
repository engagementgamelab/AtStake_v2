﻿using UnityEngine;
using System.Collections;

namespace Views {

	// After their pitch time has expired, players are given the opportunity to buy more time

	public class ExtraTime : View {

		protected override void OnInitElements () {
			if (Game.Score.CanAffordExtraTime) {
				Elements.Add ("instruction", new TextElement (
					DataManager.GetTextFromScreen (Model, "can_afford", TextVariables)));
				Elements.Add ("accept", new ButtonElement (Model.Buttons["accept"], () => {
					GotoView ("pitch");
					Game.Dispatcher.ScheduleMessage ("AcceptExtraTime", Name);
				}, "add_coin"));
				Elements.Add ("cost", new TextElement ("-" + DataManager.GetSettings().ExtraTimeCost));
			} else {
				Elements.Add ("instruction", new TextElement (
					DataManager.GetTextFromScreen (Model, "cant_afford", TextVariables)));
			}

			Elements.Add ("decline", new ButtonElement (Model.Buttons["decline"], (ButtonElement b) => {
				Game.Dispatcher.ScheduleMessage ("DeclineExtraTime", Name);
			}));
		}
	}
}