using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Decide : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarHeight = TemplateSettings.ShortBar,
					TopBarColor = Palette.Pink,
					BottomBarHeight = TemplateSettings.TallBar,
					BottomBarColor = Palette.LtTeal,
					BackgroundColor = Palette.White,
					PotEnabled = true,
					CoinsEnabled = true,
					Colors = new Dictionary<string, Color> () {
						{ "peer_list", Palette.Blue },
						{ "confirm", Palette.Orange }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "peer_list", TextStyle.LtButton },
						{ "confirm", TextStyle.LtButton }
					}
				};
			}
		}
	}
}