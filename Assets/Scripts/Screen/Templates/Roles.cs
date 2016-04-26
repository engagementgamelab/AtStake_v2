﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Roles : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("bottom_button|play") {
				TopBarHeight = TemplateSettings.TallBar,
				TopBarColor = Palette.Pink,
				BackgroundColor = Palette.White,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "role_list", TextStyle.Paragraph }
				}
			};
		}

		int roleCounter = 0;

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
			Elements["play"].Alpha = 0f;
		}

		protected override void OnUnloadView () {
			roleCounter = 0;
		}

		protected override void OnInputEnabled () {

			foreach (AvatarElementUI avatar in RoleList.ChildElements) {
				avatar.playerName.gameObject.SetActive (false);
				avatar.playerRole.gameObject.SetActive (false);
			}

			// Introduce roles over time, ending with the Decider
			Co.InvokeWhileTrue (0.25f, 0.75f, () => { return Loaded && roleCounter < RoleList.ChildElements.Count * 2; }, () => {

				RoleList.Visible = true;

				for (int i = 0; i < RoleList.ChildElements.Count*2; i ++) {

					int index = Mathf.FloorToInt ((float)i/2f);
					int roleIndex = Mathf.FloorToInt ((float)roleCounter/2f);
					AvatarElementUI t = RoleList.ChildElements[index];

					bool wasActive = t.playerName.gameObject.activeSelf;
					bool active = index <= roleIndex;
					t.gameObject.SetActive (active);

					if (active && !wasActive) 
						t.PlayAudio ();
					
					if (index == roleIndex ) {
						t.playerName.gameObject.SetActive (true);
						if (roleCounter % 2 != 0) {
							t.playerRole.gameObject.SetActive (true);
						}
					}
				}
				roleCounter ++;
			}, () => {
				Elements["play"].Animate (new UIAnimator.FadeIn (0.5f));
			});
		}
	}
}