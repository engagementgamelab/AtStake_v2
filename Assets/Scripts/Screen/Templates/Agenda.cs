using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Agenda : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "role_card", "decider_instructions") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.LtTeal,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true
			};
		}
	}
}