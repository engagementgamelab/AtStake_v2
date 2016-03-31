using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Deliberate : Template {

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
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "rc_agenda", TextStyle.Paragraph }
					}
				};
			}
		}
	}
}