using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Pot : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "coins_and_pot") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.LtTeal,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true
			};
		}

		List<TextElementUI> instructions;
		List<TextElementUI> Instructions {
			get {
				if (instructions == null) {
					instructions = new List<TextElementUI> ();
					instructions.Add ((TextElementUI)Elements["instruction1"]);
					if (LoadedElements.ContainsKey ("instruction2"))
						instructions.Add ((TextElementUI)Elements["instruction2"]);
					instructions.Add ((TextElementUI)Elements["instruction3"]);
					instructions.Add ((TextElementUI)Elements["instruction4"]);
					instructions.Add ((TextElementUI)Elements["instruction5"]);
				}
				return instructions;
			}
		}

		protected override void OnLoadView () {
			Elements["pot"].Visible = false;
			Elements["coins"].Visible = false;
			Elements["next"].Visible = false;
			foreach (TextElementUI t in Instructions) {
				t.Visible = false;
			}
		}

		protected override void OnInputEnabled () {
			Co.RepeatAscending (0.5f, 3.5f, Instructions.Count, (int i) => {

				// Wait until the view is loaded
				if (!Loaded) return;

				// Remove the previous instruction
				if (i > 0) {
					Instructions[i-1].Animate (new UIAnimator.FadeOut (0.2f, () => {
						Instructions[i-1].Visible = false;
						ShowInstruction (i);
					}));
				} else {
					ShowInstruction (i);
				}
				
			}, () => {
				if (Loaded)
					Elements["next"].Visible = true;
			});
		}

		void ShowInstruction (int index) {

			// Show the current instruction
			Instructions[index].Visible = true;
			Instructions[index].Animate (new UIAnimator.FadeIn (0.5f));

			if (Elements["instruction1"].Visible && Elements["next"].Loaded)
				FadeInElement ("coins");
			if (Elements["instruction2"].Visible)
				FadeInElement ("coins");
			if (Elements["instruction4"].Visible) 
				FadeInElement ("pot");
		}

		void FadeInElement (string id) {
			if (!Elements[id].Visible) {
				Elements[id].Visible = true;
				Elements[id].Animate (new UIAnimator.FadeIn (0.5f));
			}
		}

		protected override void OnUnloadView () {
			instructions = null;
		}
	}
}