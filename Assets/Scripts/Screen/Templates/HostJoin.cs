using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class HostJoin : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					BackgroundColor = Palette.Teal,
					Colors = new Dictionary<string, Color> () {
						{ "host", Palette.Orange },
						{ "join", Palette.Green },
						{ "logo", Palette.Transparent.White (0.5f) }
					}
				};
			}
		}
	}
}