using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class FinalScoreboard : Scoreboard {

		protected override ScreenElementUI ContinueButton {
			get { return Elements["menu"]; }
		}

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("bottom_button|menu") {
				TopBarColor = Palette.Orange,
				TopBarHeight = TemplateSettings.ShortBar,
				BackgroundColor = Palette.White,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "score_list", TextStyle.Paragraph }
				}
			};
		}
	}
}