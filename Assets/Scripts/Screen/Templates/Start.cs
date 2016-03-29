using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Start : Template {

		public override TemplateSettings Settings {
			get { 
				return new TemplateSettings () {
					BackgroundColor = Palette.Teal,
					Colors = new Dictionary<string, Color> () {
						{ "submit", Palette.Green },
						{ "logo", Palette.Transparent.White (0.5f) }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "instructions", TextStyle.LtParagraph }
					}
				};
			}
		}
	}
}