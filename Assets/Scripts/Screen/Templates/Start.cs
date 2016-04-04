using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Start : Template {

		/*public override TemplateSettings Settings {
			get { 
				return new TemplateSettings ("logo", "lt_paragraph|instructions") {
					BackgroundColor = Palette.Teal,
					Colors = new Dictionary<string, Color> () {
						{ "submit", Palette.Green }
					}
				};
			}
		}*/

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("logo", "lt_paragraph|instructions") {
				BackgroundColor = Palette.Teal,
				Colors = new Dictionary<string, Color> () {
					{ "submit", Palette.Green }
				}
			};
		}
	}
}