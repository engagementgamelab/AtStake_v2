using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Players take turns convincing the Decider that they have the best pitch
// The Decider starts a timer and the player pitches
// After the timer expires, the player is given the opportunity to buy more time and continue pitching

public class PitchScreen : GameScreen {

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

	protected override void OnInitDeciderElements () {

		Game.Dispatcher.AddListener ("AcceptExtraTime", AcceptExtraTime);
		Game.Dispatcher.AddListener ("DeclineExtraTime", DeclineExtraTime);
		peers = ShuffledPeers ();
		currentPeer = 0;
		state = State.Pitch;

		Elements.Add ("instructions", new TextElement (peers[currentPeer] + " is first up. Start the timer when they're ready!"));
		Elements.Add ("timer", new TimerButtonElement (Duration, () => {
			Game.Dispatcher.ScheduleMessage (
				"StartTimer", 
				peers[currentPeer], 
				state == State.Pitch ? "pitch" : "extra"
			);
		}));
	}

	protected override void OnInitPlayerElements () {
		CreateRoleCard (true, true, true);
	}

	protected override void OnInitElements () {
		Elements.Add ("pot", new PotElement ());
		Elements.Add ("coins", new CoinsElement ());
	}

	protected override void OnShow () {
		Game.Dispatcher.AddListener ("StartTimer", StartTimer);
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

			AddElement<TimerElement> ("timer", new TimerElement (duration, () => {
				GotoScreen ("extra_time");
			})).StartTimer ();
		}
	}

	void AcceptExtraTime (NetworkMessage msg) {
		if (IsDecider) {
			state = State.Extra;
			TimerButtonElement t = GetScreenElement<TimerButtonElement> ("timer");
			t.Reset (ExtraTimeDuration);
			t.StartTimer ();
		}
	}

	void DeclineExtraTime (NetworkMessage msg) {
		if (IsDecider) {
			state = State.Pitch;
			currentPeer ++;
			if (currentPeer < peers.Count) {
				GetScreenElement<TextElement> ("instructions")
					.SetText ("next up is " + peers[currentPeer] + ". Start the timer when they're ready!");
				GetScreenElement<TimerButtonElement> ("timer").Reset (Duration);
			} else {
				AllGotoScreen ("deliberate_instructions");
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
