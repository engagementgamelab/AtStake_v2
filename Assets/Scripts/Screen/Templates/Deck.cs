using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Deck : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					BackgroundColor = Palette.Teal,
					Colors = new Dictionary<string, Color> () {
						{ "confirm", Palette.Orange },
						{ "logo", Palette.Transparent.White (0.5f) }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "deck_list", TextStyle.Button },
						{ "client_instructions", TextStyle.LtParagraph },
						{ "host_instructions", TextStyle.LtParagraph },
						{ "confirm", TextStyle.LtButton }
					}
				};
			}
		}
	}
}