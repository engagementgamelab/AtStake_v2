using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class AgendaItem : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarColor = Palette.Orange,
					TopBarHeight = TemplateSettings.ShortBar,
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