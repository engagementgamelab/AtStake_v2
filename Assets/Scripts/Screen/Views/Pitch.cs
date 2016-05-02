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
			get { return new Dictionary<string, string> () { { "current_peer", CurrentPitcher } }; }
		}

		TimerButtonElement TimerButton {
			get { return GetScreenElement<TimerButtonElement> ("timer_button"); }
		}

		string CurrentPitcher { get { return Game.Controller.CurrentPitcher; } }
		bool IsCurrentPitcher { get { return CurrentPitcher == Name; } }
		bool IsNextPitcher { get { return Game.Controller.NextPitcher == Name; }}
		bool droppedClient = false;

		protected override void OnInitDeciderElements () {

			Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
			state = State.Pitch;

			Elements.Add ("timer_button", new TimerButtonElement (GetButton ("timer_button"), Duration, () => {
				Game.Audio.Play ("timer_start");
				Game.Dispatcher.ScheduleMessage (
					"StartTimer", 
					CurrentPitcher,
					state == State.Pitch ? "pitch" : "extra"
				);
				Elements["skip"].Active = false;
				droppedClient = false;
			}));

			// The skip button is only shown if a client was dropped (so that players don't need to wait for the timer to run down again)
			Elements.Add ("skip", new ButtonElement (GetButton ("skip"), () => {
				TimerButton.Skip ();
				droppedClient = false;
				Game.Dispatcher.ScheduleMessage ("TimerSkip");
				Elements["skip"].Active = false;
			}) { Active = droppedClient });
		}

		protected override void OnInitPlayerElements () {

			CreateRoleCard (true, true, true, false);

			string timerText = GetButton (IsCurrentPitcher ? "pitching" : "listening"); 
			TimerType type = IsCurrentPitcher ? TimerType.Pitch : TimerType.Listen;

			Elements.Add ("timer", new TimerElement (timerText, Duration, type, () => {
				if (IsCurrentPitcher) {
					GotoView ("extra_time");
					Game.Audio.Play ("alarm");
				}
			}));

			Game.Dispatcher.AddListener ("TimerSkip", TimerSkip);
		}

		protected override void OnInitElements () {
			Elements.Add ("question", new TextElement (Game.Controller.Question));
		}
		
		protected override void OnShow () {
			Game.Dispatcher.AddListener ("DeclineExtraTime", DeclineExtraTime);
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
			Game.Dispatcher.RemoveListener (TimerSkip);
		}

		void StartTimer (NetMessage msg) {

			if (IsDecider) return;

			float duration = msg.str2 == "pitch"
				? Duration
				: ExtraTimeDuration;

			// Start the timer
			TimerElement timer = GetScreenElement<TimerElement> ("timer");
			timer.Reset (duration);
			timer.StartTimer ();
		}

		void AcceptExtraTime (NetMessage msg) {
			AllGotoView ("pitch");
			state = State.Extra;
			TimerButton.Reset (ExtraTimeDuration);
			TimerButton.StartTimer ();
		}

		void DeclineExtraTime (NetMessage msg) {

			// Update player timers
			if (!IsDecider) {
				TimerElement timer = GetScreenElement<TimerElement> ("timer");
				timer.Text = GetButton (IsNextPitcher ? "pitching" : "listening");
				timer.Type = IsNextPitcher ? TimerType.Pitch : TimerType.Listen;
				timer.Reset ();
				return;
			}

			// Decider logic
			state = State.Pitch;
			Game.Controller.NextPitch ();
			if (CurrentPitcher != "") {
				AllGotoView ("pitch");
				GetScreenElement<TextElement> ("decider_instructions")
					.Text = DataManager.GetTextFromScreen (Model, "next_up", CurrentPitcherTextVariable);
				TimerButton.Reset (Duration);
			} else {
				AllGotoView ("deliberate_instructions");
			}
		}

		void TimerSkip (NetMessage msg) {
			GetScreenElement<TimerElement> ("timer").Skip ();
		}
	}
}