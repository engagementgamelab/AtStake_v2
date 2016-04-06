using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Think : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings () {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.Pink,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "rc_agenda", TextStyle.Paragraph }
				}
			};
		}
	}
}