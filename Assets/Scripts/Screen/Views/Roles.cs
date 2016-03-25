using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Models;

namespace Views {

	// Randomly assigned roles, with one of the roles being the Decider
	// Everyone sees everyone else's role

	public class Roles : View {

		ListElement<TextElement> roleList;

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("pot"));
		}

		protected override void OnInitElements () {
			roleList = new ListElement<TextElement> ();
			Elements.Add ("role_list", roleList);
		}

		protected override void OnShow () {
			foreach (PlayerRole role in Game.Controller.Roles)
				roleList.Add (role.PlayerName + "|" + role.Title, new TextElement (""));
		}
	}
}