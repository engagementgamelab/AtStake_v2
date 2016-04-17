using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Players and Decider deliberate over who has the best plan
	// Players try to get the winning plan to include their agenda items
	// After the timer expires, all players are given the opportunity to buy more time and continue deliberating
	// The game advances once all players have declined the extra time

	public class Deliberate : View {

		float Duration {
			get { return DataManager.GetSettings ().DeliberateSeconds; }
		}

		float ExtraTimeDuration {
			get { return DataManager.GetSettings ().ExtraSeconds; }
		}

		enum State { Deliberate, Extra }
		State state = State.Deliberate;

		List<string> declinedPlayers = new List<string> ();

		protected override void OnInitDeciderElements () {
			Elements.Add ("timer_button", new TimerButtonElement (GetButton ("timer_button"), Duration, () => {
				Game.Dispatcher.ScheduleMessage ("StartTimer", state == State.Deliberate ? "deliberate" : "extra");
			}, () => {
				Game.Dispatcher.ScheduleMessage ("TimeExpired");
			}));
			Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
			Game.Dispatcher.AddListener ("DeclineExtraTime", DeclineExtraTime);
		}

		protected override void OnInitPlayerElements () {
			CreateRoleCard (true, true, true);
			Elements.Add ("timer", new TimerElement (GetButton ("timer_button"), Duration, TimerType.Deliberate));
		}

		protected override void OnShow () {
			Game.Dispatcher.AddListener ("StartTimer", StartTimer);
			Game.Dispatcher.AddListener ("TimeExpired", TimeExpired);
		}

		protected override void OnHide () {
			Game.Dispatcher.RemoveListener (StartTimer);
			Game.Dispatcher.RemoveListener (TimeExpired);
			Game.Dispatcher.RemoveListener (AcceptExtraTime);
			Game.Dispatcher.RemoveListener (DeclineExtraTime);
			state = State.Deliberate;
		}

		void StartTimer (MasterMsgTypes.GenericMessage msg) {
			
			if (HasElement ("timer")) {

				float duration = msg.str1 == "deliberate"
					? Duration
					: ExtraTimeDuration;

				TimerElement timer = GetScreenElement<TimerElement> ("timer");
				timer.Reset (duration);
				timer.StartTimer ();
			}
			state = State.Extra;
		}

		void AcceptExtraTime (MasterMsgTypes.GenericMessage msg) {
			AllGotoView ("deliberate");
			declinedPlayers.Clear ();
			TimerButtonElement timer = GetScreenElement<TimerButtonElement> ("timer_button");
			timer.Reset (ExtraTimeDuration);
			timer.StartTimer ();
		}

		void DeclineExtraTime (MasterMsgTypes.GenericMessage msg) {
			if (!declinedPlayers.Contains (msg.str1)) {
				declinedPlayers.Add (msg.str1);
				if (declinedPlayers.Count == Game.Controller.PeerCount) {
					AllGotoView ("decide");
				}
			}
		}

		void TimeExpired (MasterMsgTypes.GenericMessage msg) {
			if (!IsDecider) {
				GotoView ("extra_time_deliberate");
			}
		}
	}
}