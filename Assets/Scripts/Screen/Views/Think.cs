using UnityEngine;
using System.Collections;

namespace Views {

	// Players silently brainstorm plans
	// Decider just kinda waits, awkwardly

	public class Think : View {

		float Duration {
			get { return DataManager.GetSettings ().ThinkSeconds; }
		}

		protected override void OnInitDeciderElements () {
			Elements.Add ("timer", new TimerButtonElement (Duration, () => {
				Game.Dispatcher.ScheduleMessage ("StartTimer");
			}, () => {
				AllGotoView ("pitch_instructions");	
			}));
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
}