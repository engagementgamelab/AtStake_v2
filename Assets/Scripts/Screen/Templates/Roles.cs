﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Roles : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarHeight = TemplateSettings.TallBar,
					TopBarColor = Palette.Pink,
					BackgroundColor = Palette.White,
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "role_list", TextStyle.Paragraph }
					},
					Colors = new Dictionary<string, Color> () {
						{ "next", Palette.LtBlue }
					}
				};
			}
		}

		int roleCounter = 0;

		/*ListElementUI<TextElementUI, TextElement> roleList;
		ListElementUI<TextElementUI, TextElement> RoleList {
			get {
				if (roleList == null)
					roleList = GetElement<ListElementUI<TextElementUI, TextElement>> ("role_list");
				return roleList;
			}
		}*/

		ListElementUI<AvatarElementUI, AvatarElement> roleList;
		ListElementUI<AvatarElementUI, AvatarElement> RoleList {
			get {
				if (roleList == null)
					roleList = GetElement<ListElementUI<AvatarElementUI, AvatarElement>> ("role_list");
				return roleList;
			}
		}

		protected override void OnLoadView () {
			RoleList.Visible = false;
			Elements["next"].Visible = false;
		}

		protected override void OnUnloadView () {
			roleCounter = 0;
		}

		protected override void OnInputEnabled () {

			// Introduce roles over time, ending with the Decider
			Co.InvokeWhileTrue (0.25f, 0.75f, () => { return Loaded && roleCounter < RoleList.ChildElements.Count * 2; }, () => {

				RoleList.Visible = true;

				for (int i = 0; i < RoleList.ChildElements.Count*2; i ++) {

					int index = Mathf.FloorToInt ((float)i/2f);
					int roleIndex = Mathf.FloorToInt ((float)roleCounter/2f);
					AvatarElementUI t = RoleList.ChildElements[index];

					bool active = index <= roleIndex;
					t.gameObject.SetActive (active);
					
					if (index == roleIndex) {
						string[] playerRole = t.id.Split ('|');
						t.playerName.text = playerRole[0];
						if (roleCounter % 2 != 0) {
							t.playerName.text += "\nthe " + playerRole[1];
						}
					}
				}
				roleCounter ++;
			}, () => {
				Elements["next"].Visible = true;
			});
		}
	}
}