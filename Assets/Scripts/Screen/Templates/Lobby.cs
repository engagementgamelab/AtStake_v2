using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Lobby : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("bottom_button|play") {
				TopBarHeight = TemplateSettings.TallBar,
				TopBarColor = Palette.Pink,
				BackgroundColor = Palette.White,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "title", TextStyle.Header2 }
				}
			};
		}
	}
}