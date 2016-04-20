using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class AgendaItem : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("blue_button|accept", "orange_button|reject", "lt_coins_and_pot") {
				TopBarColor = Palette.Orange,
				TopBarHeight = TemplateSettings.ShortBar,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "item", TextStyle.Header3 }
				}
			};
		}
	}
}