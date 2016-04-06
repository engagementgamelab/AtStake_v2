using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class AgendaItemReject : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button") {
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