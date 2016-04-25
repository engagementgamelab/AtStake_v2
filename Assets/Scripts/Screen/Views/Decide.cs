using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	public class Decide : View {
		
		protected override void OnInitDeciderElements () {
			Elements.Add ("peer_list", new RadioListElement (GetButton ("confirm"), (string selected) => {
				Game.Dispatcher.ScheduleMessage ("ChooseWinner", selected);
			}, Game.Controller.PeerNames));
		}

		protected override void OnShow () { Game.Dispatcher.AddListener ("ChooseWinner", ChooseWinner); }
		protected override void OnHide () { Game.Dispatcher.RemoveListener (ChooseWinner); }

		void ChooseWinner (NetMessage msg) {
			Game.Controller.SetWinner (msg.str1);
			GotoView ("winner");
		}
	}
}