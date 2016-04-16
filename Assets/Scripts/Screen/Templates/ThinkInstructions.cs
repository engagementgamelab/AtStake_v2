using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class ThinkInstructions : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "decider_instructions") {
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