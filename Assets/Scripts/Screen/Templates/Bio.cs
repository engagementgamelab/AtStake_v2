using UnityEngine;
using System.Collections;

namespace Templates {

	public class Bio : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarColor = Palette.LtTeal,
					BackgroundColor = Palette.White,
					PotEnabled = true,
					CoinsEnabled = true
				};
			}
		}
	}
}