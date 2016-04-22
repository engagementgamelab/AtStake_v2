using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class NameTaken : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("blue_button|submit") {
				TopBarHeight = TemplateSettings.TallBar,
				TopBarColor = Palette.Pink,
				BackgroundColor = Palette.White,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "connection_failed", new TextStyle () 
						{
							FontSize = 16,
							FontColor = Palette.Grey,
							FontStyle = FontStyle.Bold
						}
					}
				}
			};
		}

		bool inputFocused = false;

		protected override void OnInputEnabled () {

			InputElementUI input = GetElement<InputElementUI> ("input");
			
			Co.RunWhileTrue (() => { return Loaded; }, () => {

				bool isInputFocused = input.InputField.isFocused;

				if (inputFocused && !isInputFocused) {
					anim.Animate (new UIAnimator.Peek (0.3f, 0, () => {
						inputFocused = false;
					}));
				} else if (!inputFocused && isInputFocused) {
					anim.Animate (new UIAnimator.Peek (0.3f, 60, () => {
						inputFocused = true;
					}));
				}
			});
		}
	}
}