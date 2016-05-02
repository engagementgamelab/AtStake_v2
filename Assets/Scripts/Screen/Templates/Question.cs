using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Question : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "decider_instructions", "coins_and_pot", "question") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.LtTeal,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true,
				Colors = new Dictionary<string, Color> () {
					{ "separator", Palette.LtTeal }
				},
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "round", new TextStyle () 
						{
							FontSize = 24,
							FontColor = Palette.Orange
						}
					}
				}
			};
		}
	}
}