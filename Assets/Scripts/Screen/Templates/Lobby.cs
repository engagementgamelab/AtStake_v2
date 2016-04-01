using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Lobby : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarHeight = TemplateSettings.TallBar,
					TopBarColor = Palette.Pink,
					BackgroundColor = Palette.White,
					Colors = new Dictionary<string, Color> () {
						{ "play", Palette.LtBlue }
					},
					TextStyles = new Dictionary<string, TextStyle> () {
						{ "title", TextStyle.Header2 },
						{ "play", TextStyle.LargeButton }
					}
				};
			}
		}
	}
}