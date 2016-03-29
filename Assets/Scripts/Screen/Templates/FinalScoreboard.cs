using UnityEngine;
using System.Collections;

namespace Templates {

	public class FinalScoreboard : Scoreboard {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarColor = Palette.Orange,
					BackgroundColor = Palette.White,
					BackgroundImage = "applause-bg"
				};
			}
		}
	}
}