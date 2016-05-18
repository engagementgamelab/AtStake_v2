using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class SocketDisconnected : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("orange_button|menu", "lt_paragraph|instructions") {
				BackgroundColor = Palette.Teal
			};
		}
	}
}