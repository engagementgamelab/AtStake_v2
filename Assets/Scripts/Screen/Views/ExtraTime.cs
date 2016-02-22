﻿using UnityEngine;
using System.Collections;

namespace Views {

	// After their pitch time has expired, players are given the opportunity to buy more time

	public class ExtraTime : View {

		protected override void OnInitElements () {
			if (Game.Score.CanAffordExtraTime) {
				Elements.Add ("can_afford", new TextElement (
					DataManager.GetTextFromScreen (Model, "can_afford", TextVariables)));
				Elements.Add ("accept", new ButtonElement (Model.Buttons["accept"], () => {
					GotoView ("pitch");
					Game.Dispatcher.ScheduleMessage ("AcceptExtraTime", Name);
				}));
			} else {
				Elements.Add ("cant_afford", new TextElement (
					DataManager.GetTextFromScreen (Model, "can_afford", TextVariables)));
			}

			Elements.Add ("decline", new ButtonElement (Model.Buttons["decline"], () => {
				GotoView ("pitch");
				Game.Dispatcher.ScheduleMessage ("DeclineExtraTime");	
			}));
		}
	}
}