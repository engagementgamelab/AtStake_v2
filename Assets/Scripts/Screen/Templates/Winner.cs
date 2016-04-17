﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Winner : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "lt_coins_and_pot") {
				TopBarColor = Palette.Orange,
				TopBarHeight = TemplateSettings.ShortBar,
				BottomBarHeight = TemplateSettings.MediumBar,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true
			};
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