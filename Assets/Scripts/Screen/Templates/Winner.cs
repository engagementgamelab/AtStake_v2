using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Winner : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarColor = Palette.Orange,
					TopBarHeight = TemplateSettings.ShortBar,
					BackgroundColor = Palette.White,
					PotEnabled = true,
					CoinsEnabled = true,
					Colors = new Dictionary<string, Color> () {
						{ "next", Palette.Orange }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "next", TextStyle.LtButton }
					}
				};
			}
		}

		protected override void OnLoadView () {
			Elements["winner_name"].Visible = false;
			Elements["next"].Visible = false;
		}

		protected override void OnInputEnabled () {
			Co.WaitForSeconds (1f, () => {
				Elements["winner_name"].Visible = true;
				Elements["next"].Visible = true;
			});
		}
	}
}