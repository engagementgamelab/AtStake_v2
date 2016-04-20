using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Pitch : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "role_card", "coins_and_pot") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.Pink,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true
			};
		}
	}
}