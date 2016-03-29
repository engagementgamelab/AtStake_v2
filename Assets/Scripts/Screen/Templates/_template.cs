using UnityEngine;
using System.Collections;

namespace Templates {

	public class _template : Template {

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