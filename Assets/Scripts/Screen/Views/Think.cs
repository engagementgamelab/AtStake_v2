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
			Elements.Add ("timer_button", new TimerButtonElement (GetButton ("timer_button"), Duration, () => {
				Game.Dispatcher.ScheduleMessage ("StartTimer");
				Game.Audio.Play ("timer_start");
			}, () => {
				AllGotoView ("pitch_instructions");	
				Game.Audio.Play ("alarm");
			}));
		}

		protected override void OnInitPlayerElements () {
			CreateRoleCard (true, true, true);
			Elements.Add ("timer", new TimerElement (GetButton ("timer_button"), Duration, TimerType.Think));
		}

		protected override void OnShow () {
			Game.Dispatcher.AddListener ("StartTimer", StartTimer);
		}

		protected override void OnHide () {
			Game.Dispatcher.RemoveListener (StartTimer);
		}

		void StartTimer (NetMessage msg) {
			if (HasElement ("timer")) {
				GetScreenElement<TimerElement> ("timer").StartTimer ();
			}
		}
	}
}