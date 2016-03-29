﻿using UnityEngine;
using System.Collections;

namespace Templates {

	public class AgendaItemAccept : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarColor = Palette.Orange,
					BackgroundColor = Palette.White,
					BackgroundImage = "applause-bg",
					PotEnabled = true,
					CoinsEnabled = true
				};
			}
		}
	}
}