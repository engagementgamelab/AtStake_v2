using UnityEngine;
using System.Collections;

namespace Views {

	// Players silently brainstorm plans
	// Decider just kinda waits, awkwardly

	public class Think : View {

		float Duration {
			get { return DataManager.GetSettings ().ThinkSeconds; }
		}

		bool droppedClient = false;

		protected override void OnInitDeciderElements () {

			Elements.Add ("timer_button", new TimerButtonElement (GetButton ("timer_button_decider"), Duration, () => {
				Game.Dispatcher.ScheduleMessage ("StartTimer");
				Game.Audio.Play ("timer_start");
			}, () => {
				Advance ();
				Game.Audio.Play ("alarm");
			}));

			// The skip button is only shown if a client was dropped (so that players don't need to wait for the timer to run down again)
			Elements.Add ("skip", new ButtonElement (GetButton ("skip"), () => {
				Advance ();
			}) { Active = droppedClient });
		}

		protected override void OnInitPlayerElements () {
			CreateRoleCard (true, true, true, false);
			Elements.Add ("timer", new TimerElement (GetButton ("timer_button"), Duration, TimerType.Think));
		}

		protected override void OnInitElements () {
			Elements.Add ("question", new TextElement (Game.Controller.Question));
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

		void Advance () {
			AllGotoView ("pitch_instructions");
			droppedClient = false;
		}

		public override void OnClientDropped () {
			base.OnClientDropped ();
			droppedClient = true;
		}
	}
}