using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Bio : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarColor = Palette.LtTeal,
					TopBarHeight = TemplateSettings.ShortBar,
					BackgroundColor = Palette.White,
					PotEnabled = true,
					CoinsEnabled = true,
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "next", TextStyle.LtButton }
					},
					Colors = new Dictionary<string, Color> () {
						{ "next", Palette.Orange }
					}
				};
			}
		}
	}
}