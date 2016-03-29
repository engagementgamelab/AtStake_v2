using UnityEngine;
using System.Collections;

namespace Templates {

	public class Roles : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarHeight = TemplateSettings.TallBar,
					TopBarColor = Palette.Pink,
					BackgroundColor = Palette.White
				};
			}
		}

		int roleCounter = 0;

		ListElementUI<TextElementUI, TextElement> roleList;
		ListElementUI<TextElementUI, TextElement> RoleList {
			get {
				if (roleList == null)
					roleList = GetElement<ListElementUI<TextElementUI, TextElement>> ("role_list");
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
					TextElementUI t = RoleList.ChildElements[index];

					bool active = index <= roleIndex;
					t.gameObject.SetActive (active);
					
					if (index == roleIndex) {
						string[] playerRole = t.id.Split ('|');
						t.Text.text = playerRole[0];
						if (roleCounter % 2 != 0) {
							t.Text.text += " the " + playerRole[1];
							/*if (playerRole[1] == "Decider")
								t.Style = FontStyle.Bold;*/
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