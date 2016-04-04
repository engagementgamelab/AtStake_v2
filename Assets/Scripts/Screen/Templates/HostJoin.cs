using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class HostJoin : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("logo", "lt_paragraph|welcome|error", "orange_button|host", "green_button|join") {
				BackgroundColor = Palette.Teal
			};
		}
	}
}