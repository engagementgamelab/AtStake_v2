using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Bio : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button") {
				TopBarColor = Palette.LtTeal,
				TopBarHeight = TemplateSettings.ShortBar,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "rc_title", TextStyle.Header2 }
				},
				Colors = new Dictionary<string, Color> () {
					{ "decider_instructions", Palette.Grey }
				}
			};
		}
	}
}