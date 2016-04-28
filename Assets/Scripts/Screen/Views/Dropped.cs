using UnityEngine;
using System.Collections;

namespace Views {

	public class Dropped : View {

		protected override void OnInitElements () {
			Elements.Add ("menu", new ButtonElement (Model.Buttons["menu"], () => { 
				Game.EndGame ();
			}));
			Elements.Add ("timeout", new TextElement (GetText ("timeout")));
		}

		public override void OnDisconnect () {
			GotoView ("start");
		}
	}
}