using UnityEngine;
using System.Collections;

namespace Templates {

	public class Lobby : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarHeight = TemplateSettings.TallBar,
					TopBarColor = Palette.Pink,
					BackgroundColor = Palette.White
				};
			}
		}
	}
}