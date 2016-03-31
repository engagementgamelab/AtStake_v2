using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Models;

namespace Views {

	// Randomly assigned roles, with one of the roles being the Decider
	// Everyone sees everyone else's role

	public class Roles : View {

		ListElement<AvatarElement> roleList;

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("pot"));
		}

		protected override void OnInitElements () {
			roleList = new ListElement<AvatarElement> ();
			Elements.Add ("role_list", roleList);
		}

		protected override void OnShow () {

			string deciderName = "";

			foreach (PlayerRole role in Game.Controller.Roles) {
				string playerName = role.PlayerName;
				string title = role.Title;
				if (title != "Decider") {
					roleList.Add (playerName + "|" + title, new AvatarElement ("", Game.Manager.Players[playerName].Avatar));
				} else {
					deciderName = playerName;
				}
			}

			roleList.Add (deciderName + "|Decider", new AvatarElement ("", Game.Manager.Players[deciderName].Avatar));
		}
	}
}