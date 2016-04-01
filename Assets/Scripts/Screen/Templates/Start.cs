﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Start : Template {

		public override TemplateSettings Settings {
			get { 
				return new TemplateSettings ("logo", "lt_paragraph|instructions") {
					BackgroundColor = Palette.Teal,
					Colors = new Dictionary<string, Color> () {
						{ "submit", Palette.Green }
					}/*,
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "instructions", TextStyle.LtParagraph }
					}*/
				};
			}
		}
	}
}