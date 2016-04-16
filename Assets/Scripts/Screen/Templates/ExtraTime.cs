using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class ExtraTime : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("blue_button|accept", "orange_button|decline") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.Pink,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "cost", TextStyle.Coin }
				}
			};
		}
	}
}