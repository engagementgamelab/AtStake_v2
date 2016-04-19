using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Start : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("logo", "lt_paragraph|instructions", "green_button|submit") {
				BackgroundColor = Palette.Teal,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "connection_failed", new TextStyle () 
						{
							FontSize = 16,
							FontColor = Palette.White,
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