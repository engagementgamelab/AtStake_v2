using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Agenda : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.LtTeal,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true,
				Colors = new Dictionary<string, Color> () {
					{ "agenda_title", Palette.LtTeal }
				},
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "rc_agenda", TextStyle.Paragraph }
				}
			};
		}
	}
}