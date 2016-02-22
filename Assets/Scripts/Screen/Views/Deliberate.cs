using UnityEngine;
using System.Collections;

namespace Views {

	// Players and Decider deliberate over who has the best plan
	// Players try to get the winning plan to include their agenda items

	public class Deliberate : View {

		float Duration {
			get { return DataManager.GetSettings ().DeliberateSeconds; }
		}

		protected override void OnInitDeciderElements () {
			Elements.Add ("timer", new TimerButtonElement (Duration, () => {
				Game.Dispatcher.ScheduleMessage ("StartTimer");
			}, () => {
				AllGotoView ("decide");
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