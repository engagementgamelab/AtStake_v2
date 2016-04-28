using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Dropped : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("orange_button|menu", "lt_paragraph|instructions|timeout") {
				BackgroundColor = Palette.Teal
			};
		}

		protected override void OnLoadView () {
			Elements["timeout"].Alpha = 0;
			Elements["menu"].Alpha = 0;
		}

		protected override void OnInputEnabled () {
			Co.WaitForSeconds (5f, () => {
				Elements["timeout"].Animate (new UIAnimator.FadeIn (0.5f, () => {
					Elements["menu"].Animate (new UIAnimator.FadeIn (0.5f));
				}));
			});
		}
	}
}
