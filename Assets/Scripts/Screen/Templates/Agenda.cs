using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Agenda : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "role_card", "decider_instructions", "coins_and_pot") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.LtTeal,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true
			};
		}

		protected override void OnLoadView () {
			TextElementUI r0;
			TextElementUI r1;
			if (TryGetElement<TextElementUI> ("rc_reward0", out r0))
				r0.Text.fontSize = 20;
			if (TryGetElement<TextElementUI> ("rc_reward1", out r1))
				r1.Text.fontSize = 20;
		}
	}
}