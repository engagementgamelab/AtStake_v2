using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class HostJoin : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("logo", "lt_paragraph|welcome|error") {
				BackgroundColor = Palette.Teal,
				Colors = new Dictionary<string, Color> () {
					{ "host", Palette.Orange },
					{ "join", Palette.Green }
				}
			};
		}
	}
}