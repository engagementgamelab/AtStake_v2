using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class HostJoin : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings ("logo") {
					BackgroundColor = Palette.Teal,
					Colors = new Dictionary<string, Color> () {
						{ "host", Palette.Orange },
						{ "join", Palette.Green }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "welcome", TextStyle.LtParagraph },
						{ "error", TextStyle.LtParagraph }
					}
				};
			}
		}
	}
}