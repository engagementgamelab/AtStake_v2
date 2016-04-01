using UnityEngine;
using System.Collections;

namespace Views {

	// Decider sees a script to read out loud to players
	// Players see their bio (a description of their role)

	public class Bio : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("agenda"));
		}

		protected override void OnInitPlayerElements () {
			Elements.Add ("avatar", new ImageElement (AssetLoader.GetAvatarFilename (Game.Controller.Avatar)));
			CreateRoleCard (true, true, false);
		}
	}
}