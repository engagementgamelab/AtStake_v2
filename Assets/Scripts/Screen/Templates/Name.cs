using UnityEngine;
using System.Collections;

// deprecated

namespace Templates {

	public class Name : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarEnabled = true,
					TopBarColor = Palette.Orange,
					BackgroundColor = Palette.White,
					BackgroundImage = "applause-bg"
				};
			}
		}
	}
}