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

		List<string> peers;
		int currentPeer;

		Dictionary<string, string> CurrentPeerTextVariable {
			get { return new Dictionary<string, string> () { { "current_peer", peers[currentPeer] } }; }
		}

		protected override void OnInitDeciderElements () {

			Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
			Game.Dispatcher.AddListener ("DeclineExtraTime", DeclineExtraTime);
			peers = ShuffledPeers ();
			currentPeer = 0;
			state = State.Pitch;

			Elements.Add ("timer_button", new TimerButtonElement (Duration, () => {
				Game.Dispatcher.ScheduleMessage (
					"StartTimer", 
					peers[currentPeer], 
					state == State.Pitch ? "pitch" : "extra"
				);
			}));
		}

		protected override void OnInitPlayerElements () {
			CreateRoleCard (true, true, true);
			Elements.Add ("timer", new TimerElement (Duration, () => {
				GotoView ("extra_time");
			}) { Active = false });
		}

		protected override void OnShow () {
			Game.Dispatcher.AddListener ("StartTimer", StartTimer);
			if (HasElement ("decider_instructions")) {
				GetScreenElement<TextElement> ("decider_instructions")
					.Text = DataManager.GetTextFromScreen (Model, "first_up", CurrentPeerTextVariable);
			}
		}

		protected override void OnHide () {
			Game.Dispatcher.RemoveListener (StartTimer);
		}

		void StartTimer (NetworkMessage msg) {

			if (msg.str1 == Name) {

				float duration;

				if (msg.str2 == "pitch") {
					duration = Duration;
					Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
					Game.Dispatcher.AddListener ("DeclineExtraTime", DeclineExtraTime);
				} else {
					duration = ExtraTimeDuration;
				}

				TimerElement timer = GetScreenElement<TimerElement> ("timer");
				timer.Active = true;
				timer.Reset (duration);
				timer.StartTimer ();
			}
		}

		void AcceptExtraTime (NetworkMessage msg) {
			if (IsDecider) {
				AllGotoView ("pitch");
				state = State.Extra;
				TimerButtonElement t = GetScreenElement<TimerButtonElement> ("timer_button");
				t.Reset (ExtraTimeDuration);
				t.StartTimer ();
			}
		}

		void DeclineExtraTime (NetworkMessage msg) {
			if (IsDecider) {
				state = State.Pitch;
				currentPeer ++;
				if (currentPeer < peers.Count) {
					AllGotoView ("pitch");
					GetScreenElement<TextElement> ("decider_instructions")
						.Text = DataManager.GetTextFromScreen (Model, "next_up", CurrentPeerTextVariable);
					GetScreenElement<TimerButtonElement> ("timer_button").Reset (Duration);
				} else {
					AllGotoView ("deliberate_instructions");
				}
			} else {
				Game.Dispatcher.RemoveListener (AcceptExtraTime);
				Game.Dispatcher.RemoveListener (DeclineExtraTime);
			}
		}

		List<string> ShuffledPeers () {
			List<string> peers = Game.Manager.PeerNames;

			int[] randomIndices = new int[peers.Count];
			List<int> peerIndices = new List<int> ();
			
			// Generate a list of numbers iterating by 1
			for (int i = 0; i < peers.Count; i ++)
				peerIndices.Add (i);
			
			// Randomly assign the numbers to an array
			for (int i = 0; i < randomIndices.Length; i ++) {
				int r = Random.Range (0, peerIndices.Count);
				randomIndices[i] = peerIndices[r];
				peerIndices.Remove (peerIndices[r]);
			}

			List<string> newPeers = new List<string> ();
			for (int i = 0; i < randomIndices.Length; i ++) {
				newPeers.Add (peers[randomIndices[i]]);
			}
			return newPeers;
		}
	}
}