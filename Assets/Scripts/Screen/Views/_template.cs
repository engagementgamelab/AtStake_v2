using UnityEngine;
using System.Collections;

namespace Views {

	public class _template : View {

		protected override void OnInitDeciderElements () {
			// Only the Decider
		}

		protected override void OnInitPlayerElements () {
			// All players except the Decider
		}

		protected override void OnInitHostElements () {
			// Only the host
		}

		protected override void OnInitClientElements () {
			// Only the clients (not the host)
		}

		protected override void OnInitElements () {
			// Everyone
		}

		protected override void OnShow () {
			// When the view is loaded
		}

		protected override void OnHide () {
			// When the view is unloaded
		}
	}
}