﻿using UnityEngine;
using System.Collections;

namespace Views {

	public class Disconnected : View {

		protected override void OnInitElements () {
			Elements.Add ("menu", new ButtonElement (Model.Buttons["menu"], () => { 
				Game.EndGame ();
				GotoView ("start"); 
			}));
		}

		public override void OnDisconnect () {}
	}
}