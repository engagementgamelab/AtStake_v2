using UnityEngine;
using System.Collections;

namespace Templates {

	public class Winner : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarEnabled = true,
					TopBarColor = Palette.Orange,
					BackgroundColor = Palette.White,
					BackgroundImage = "applause-bg",
					PotEnabled = true,
					CoinsEnabled = true
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