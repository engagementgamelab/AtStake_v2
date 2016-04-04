using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Decide : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("orange_button|confirm", "blue_button|peer_list") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.Pink,
				BottomBarHeight = TemplateSettings.TallBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true/*,
				Colors = new Dictionary<string, Color> () {
					{ "peer_list", Palette.Blue },
					{ "confirm", Palette.Orange }
				},
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "peer_list", TextStyle.LtButton },
					{ "confirm", TextStyle.LtButton }
				}*/
			};
		}
	}
}