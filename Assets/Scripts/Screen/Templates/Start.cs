using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Start : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("logo", "lt_paragraph|instructions", "green_button|submit") {
				BackgroundColor = Palette.Teal
			};
		}
	}
}