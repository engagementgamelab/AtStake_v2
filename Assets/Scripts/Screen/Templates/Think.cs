using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Think : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("role_card", "coins_and_pot", "orange_button|skip", "question") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.Pink,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "decider_instructions", TextStyle.Header2 }
				}
			};
		}
	}
}