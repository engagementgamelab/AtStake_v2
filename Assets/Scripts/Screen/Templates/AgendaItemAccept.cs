using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class AgendaItemAccept : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarColor = Palette.Orange,
					TopBarHeight = TemplateSettings.ShortBar,
					BackgroundColor = Palette.White,
					PotEnabled = true,
					CoinsEnabled = true,
					Colors = new Dictionary<string, Color> () {
						{ "next", Palette.Orange }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "next", TextStyle.LtButton }
					}
				};
			}
		}
	}
}