using UnityEngine;
using System.Collections;

namespace Templates {

	public class AgendaItemInstructions : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "decider_instructions") {
				TopBarColor = Palette.Orange,
				TopBarHeight = TemplateSettings.ShortBar,
				BackgroundColor = Palette.White,
				BottomBarHeight = TemplateSettings.MediumBar,
				PotEnabled = true,
				CoinsEnabled = true
			};
		}
	}
}