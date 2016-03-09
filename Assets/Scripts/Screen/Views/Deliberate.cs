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

		List<string> declinedPlayers = new List<string> ();

		protected override void OnInitDeciderElements () {
			Elements.Add ("timer_button", new TimerButtonElement (Duration, () => {
				Game.Dispatcher.ScheduleMessage ("StartTimer");
			}, () => {
				Game.Dispatcher.ScheduleMessage ("TimeExpired");
			}));
			Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
			Game.Dispatcher.AddListener ("DeclineExtraTime", DeclineExtraTime);
		}

		protected override void OnInitPlayerElements () {
			CreateRoleCard (true, true, true);
			Elements.Add ("timer", new TimerElement (Duration));
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
		}

		void StartTimer (MasterMsgTypes.GenericMessage msg) {
			if (HasElement ("timer")) {
				TimerElement timer = GetScreenElement<TimerElement> ("timer");
				timer.Reset ();
				timer.StartTimer ();
			}
		}

		void AcceptExtraTime (MasterMsgTypes.GenericMessage msg) {
			AllGotoView ("deliberate");
			declinedPlayers.Clear ();
			TimerButtonElement timer = GetScreenElement<TimerButtonElement> ("timer_button");
			timer.Reset ();
			timer.StartTimer ();
		}

		void DeclineExtraTime (MasterMsgTypes.GenericMessage msg) {
			if (!declinedPlayers.Contains (msg.str1)) {
				declinedPlayers.Add (msg.str1);
				if (declinedPlayers.Count == Game.Manager.Peers.Count) {
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