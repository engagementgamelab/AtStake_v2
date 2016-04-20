using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class AgendaItemAccept : Template {

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
	}
}