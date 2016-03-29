using UnityEngine;
using System.Collections;

// deprecated

namespace Templates {

	public class Name : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					BackgroundColor = Palette.White,
					BackgroundImage = "applause-bg"
				};
			}
		}
	}
}