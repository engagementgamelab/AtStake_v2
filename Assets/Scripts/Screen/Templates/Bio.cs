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
						{ "next", TextStyle.LtButton },
						{ "rc_title", TextStyle.Header2 }
					},
					Colors = new Dictionary<string, Color> () {
						{ "decider_instructions", Palette.Grey },
						{ "next", Palette.Orange }
					}
				};
			}
		}
	}
}