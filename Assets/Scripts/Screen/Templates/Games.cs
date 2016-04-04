using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Games : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("logo", "lt_paragraph|searching|client_instructions", "orange_button|confirm") {
				BackgroundColor = Palette.Teal,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "game_list", TextStyle.Button }
				}
			};
		}
	}
}