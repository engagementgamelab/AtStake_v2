using UnityEngine;
using System.Collections;

namespace Views {

	// Players silently brainstorm plans
	// Decider just kinda waits, awkwardly, and thinks about what they're doing with their life. The paradox of life is that it seems so short, and yet it's the longest thing you'll ever do. Is sitting and silently looking at a timer really the best use of time? Or, is silent reflection perhaps the BEST use of time? All of this really depends on your "goals" going into silent reflection (if there should even be any goals, for that matter). Does it even make sense to try to figure out "what to do with your life" or is that a false dilemma promoted by a culture that insists there must be meaning lurking behind everything? When you think about it it's actually a pretty selfish notion - "what to do with your life"/"why are you here" - if every decision you make is in service to answering these questions, you're neglecting everyone around you and validating the delusion that your own personal experience is The Most Important Thing. If you really want to find tangible meaning in life, I'd argue that the answer probably involves service to others and NOT prioritizing the search for "what makes you happy." Happiness behaves kinda like an eye floater anyways. But so once the time has elapsed, the view automatically advances and the Decider listens to everyone's pitches.

	public class Think : View {

		float Duration {
			get { return DataManager.GetSettings ().ThinkSeconds; }
		}

		bool droppedClient = false;

		TimerElement timerElPlayer;
		TimerButtonElement timerElDecider;

		protected override void OnInitDeciderElements () {

			timerElDecider = new TimerButtonElement (GetButton ("timer_button_decider"), Duration, () => {
													Game.Dispatcher.ScheduleMessage ("StartTimer");
													Game.Audio.Play ("timer_start");
												}, () => {
													Advance ();
													Game.Audio.Play ("alarm");
												});
			Elements.Add ("timer_button", timerElDecider);

			// The skip button is only shown if a client was dropped (so that players don't need to wait for the timer to run down again)
			Elements.Add ("skip", new ButtonElement (GetButton ("skip"), () => {
				Advance ();
			}) { Active = droppedClient });
		}

		protected override void OnInitPlayerElements () {
			CreateRoleCard (true, true, true, false);

			timerElPlayer = new TimerElement (GetButton ("timer_button"), Duration, TimerType.Think);
			Elements.Add ("timer", timerElPlayer);
		}

		protected override void OnInitElements () {
			Elements.Add ("question", new TextElement (Game.Controller.Question));
		}

		protected override void OnShow () {
			Game.Dispatcher.AddListener ("StartTimer", StartTimer);
			Game.Dispatcher.AddListener ("UpdateDuration", RefreshTimer);
		}
		
		protected override void OnHide () {
			Game.Dispatcher.RemoveListener (StartTimer);
		}

		void StartTimer (NetMessage msg) {
			if (HasElement ("timer")) {
				GetScreenElement<TimerElement> ("timer").StartTimer ();
			}
		}

		void RefreshTimer (NetMessage msg) {
			// Game.Controller.SetWinner (msg.str1);
			// GotoView ("winner");
			Debug.Log("RefreshTimer: " + msg.flVal);

			if(timerElPlayer != null) {

				timerElPlayer.Progress = msg.flVal;
				timerElPlayer.Reset(msg.flVal);
				timerElPlayer.StartTimer(msg.flVal);
			
			}
			if(timerElDecider != null) {

				timerElDecider.Progress = msg.flVal;
				timerElDecider.Reset(msg.flVal);
			
			}
		}

		void Advance () {
			AllGotoView ("pitch_instructions");
			droppedClient = false;
		}

		public override void Refresh () {
			base.Refresh ();
		}

		public override void OnClientDropped () {

			SyncElapsedTime();

			base.OnClientDropped ();
			droppedClient = false;
			
			Elements.Add ("reconnected", new TextElement (DataManager.GetTextFromScreen (Model, "client_reconnected")));
		}

		public override void OnClientsReconnected () {
			Debug.Log ("RECONNECTED THINK!!!");
			base.OnClientsReconnected ();
			droppedClient = true;
		}
	}
}