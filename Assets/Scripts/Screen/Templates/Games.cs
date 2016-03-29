using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Games : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					BackgroundColor = Palette.Teal,
					Colors = new Dictionary<string, Color> () {
						{ "logo", Palette.Transparent.White (0.5f) }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "client_instructions", TextStyle.LtParagraph },
						{ "searching", TextStyle.LtParagraph }
					}
				};
			}
		}
	}
}