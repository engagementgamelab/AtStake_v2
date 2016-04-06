using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Decide : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("orange_button|confirm", "blue_button|peer_list") {
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