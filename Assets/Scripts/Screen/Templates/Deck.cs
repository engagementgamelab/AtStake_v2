using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Deck : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("logo", "orange_button|confirm", "lt_paragraph|client_instructions|host_instructions") {
				BackgroundColor = Palette.Teal,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "deck_list", TextStyle.Button }
				}
			};
		}
	}
}