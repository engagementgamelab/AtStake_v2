using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Agenda : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarHeight = TemplateSettings.ShortBar,
					TopBarColor = Palette.LtTeal,
					BottomBarHeight = TemplateSettings.TallBar,
					BottomBarColor = Palette.LtTeal,
					BackgroundColor = Palette.White,
					PotEnabled = true,
					CoinsEnabled = true,
					Colors = new Dictionary<string, Color> () {
						{ "agenda_title", Palette.LtTeal }
					}
				};
			}
		}
	}
}