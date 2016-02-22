﻿using UnityEngine;
using System.Collections;

namespace Templates {

	public class Start : TemplateContent {

		public override TemplateSettings Settings {
			get { 
				return new TemplateSettings () {
					TopBarEnabled = true,
					TopBarColor = Palette.Orange,
					BackgroundColor = Palette.White,
					BackgroundImage = "applause-bg"
				};
			}
		}
	}
}