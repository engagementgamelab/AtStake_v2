using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class ExtraTime : Template {

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
						{ "accept", Palette.Blue },
						{ "reject", Palette.Orange }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "accept", TextStyle.LtButton },
						{ "reject", TextStyle.LtButton }
					}
				};
			}
		}
	}
}