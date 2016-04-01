using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Games : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings ("logo") {
					BackgroundColor = Palette.Teal,
					Colors = new Dictionary<string, Color> () {
						{ "confirm", Palette.Orange }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "client_instructions", TextStyle.LtParagraph },
						{ "searching", TextStyle.LtParagraph },
						{ "game_list", TextStyle.Button }
					}
				};
			}
		}
	}
}