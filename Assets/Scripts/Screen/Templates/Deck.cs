using UnityEngine;
using System.Collections;

namespace Templates {

	public class Deck : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					BackgroundColor = Palette.Teal/*,
					Colors = new Dictionary<string, Color> () {
						{ "confirm", Palette.Orange }
					}*/
				};
			}
		}
	}
}