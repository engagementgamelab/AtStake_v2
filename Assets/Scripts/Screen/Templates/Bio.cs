using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Bio : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "decider_instructions", "coins_and_pot") {
				TopBarColor = Palette.LtTeal,
				TopBarHeight = TemplateSettings.ShortBar,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "rc_title", TextStyle.Header2 }
				}
			};
		}
	}
}