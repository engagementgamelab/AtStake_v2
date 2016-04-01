using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Players take turns convincing the Decider that they have the best pitch
	// The Decider starts a timer and the player pitches
	// After the timer expires, the player is given the opportunity to buy more time and continue pitching

	public class Pitch : View {

		float Duration {
			get { return DataManager.GetSettings ().PitchSeconds; }
		}

		float ExtraTimeDuration {
			get { return DataManager.GetSettings ().ExtraSeconds; }
		}

		enum State { Pitch, Extra }
		State state = State.Pitch;

		Dictionary<string, string> CurrentPitcherTextVariable {
			get { return new Dictionary<string, string> () { { "current_peer", Game.Controller.CurrentPitcher } }; }
		}

		protected override void OnInitDeciderElements () {

			Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
			Game.Dispatcher.AddListener ("DeclineExtraTime", DeclineExtraTime);
			state = State.Pitch;

			Elements.Add ("timer_button", new TimerButtonElement (GetButton ("timer_button"), Duration, () => {
				Game.Dispatcher.ScheduleMessage (
					"StartTimer", 
					Game.Controller.CurrentPitcher,
					state == State.Pitch ? "pitch" : "extra"
				);
			}));
		}

		protected override void OnInitPlayerElements () {
			CreateRoleCard (true, true, true);
			Elements.Add ("timer", new TimerElement (GetButton ("timer_button"), Duration, () => {
				GotoView ("extra_time");
			}) { Active = false });
		}

		protected override void OnShow () {
			Game.Dispatcher.AddListener ("StartTimer", StartTimer);
			if (HasElement ("decider_instructions")) {
				GetScreenElement<TextElement> ("decider_instructions")
					.Text = DataManager.GetTextFromScreen (Model, "first_up", CurrentPitcherTextVariable);
			}
		}

		protected override void OnHide () {
			Game.Dispatcher.RemoveListener (StartTimer);
			Game.Dispatcher.RemoveListener (AcceptExtraTime);
			Game.Dispatcher.RemoveListener (DeclineExtraTime);
		}

		void StartTimer (MasterMsgTypes.GenericMessage msg) {

			// Check if it's this player's turn to pitch
			if (msg.str1 == Name) {

				// Set the timer duration based on the pitch state
				float duration = msg.str2 == "pitch"
					? Duration
					: ExtraTimeDuration;

				// Start the timer
				TimerElement timer = GetScreenElement<TimerElement> ("timer");
				timer.Active = true;
				timer.Reset (duration);
				timer.StartTimer ();
			}
		}

		void AcceptExtraTime (MasterMsgTypes.GenericMessage msg) {
			AllGotoView ("pitch");
			state = State.Extra;
			TimerButtonElement t = GetScreenElement<TimerButtonElement> ("timer_button");
			t.Reset (ExtraTimeDuration);
			t.StartTimer ();
		}

		void DeclineExtraTime (MasterMsgTypes.GenericMessage msg) {
			state = State.Pitch;
			Game.Controller.NextPitch ();
			if (Game.Controller.CurrentPitcher != "") {
				AllGotoView ("pitch");
				GetScreenElement<TextElement> ("decider_instructions")
					.Text = DataManager.GetTextFromScreen (Model, "next_up", CurrentPitcherTextVariable);
				GetScreenElement<TimerButtonElement> ("timer_button").Reset (Duration);
			} else {
				AllGotoView ("deliberate_instructions");
			}
		}
	}
}