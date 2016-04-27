using UnityEngine;
using System.Collections;

namespace Views {

	public class Dropped : View {

		protected override void OnInitElements () {
			Elements.Add ("menu", new ButtonElement (Model.Buttons["menu"], () => { 
				Game.EndGame ();
			}));
		}

		public override void OnDisconnect () {
			GotoView ("start");
		}
	}
}