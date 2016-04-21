﻿using UnityEngine;
using System.Collections;

namespace Views {

	public class Winner : View {

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("agenda_item_instructions"));
		}

		protected override void OnInitElements () {
			Elements.Add ("winner", new TextElement (DataManager.GetTextFromScreen (Model, "winner")));
			Elements.Add ("winner_name", new TextElement (Game.Controller.WinnerName + "!"));
			Elements.Add ("avatar", new ImageElement (AssetLoader.GetAvatarFilename (Game.Controller.GetAvatarForPlayer (Game.Controller.WinnerName))));
			Elements.Add ("coins_won", new TextElement ("+" + Game.Score.Pot + " coins!"));
		}

		protected override void OnShow () {
			Game.Score.AddWinnings ();
		}
	}
}