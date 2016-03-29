using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Start : Template {

		public override TemplateSettings Settings {
			get { 
				return new TemplateSettings () {
					BackgroundColor = Palette.Teal,
					ButtonColors = new Dictionary<string, Color> () {
						{ "submit", Palette.Green }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "instructions", TextStyle.LtParagraph }
					}
				};
			}
		}
	}
}