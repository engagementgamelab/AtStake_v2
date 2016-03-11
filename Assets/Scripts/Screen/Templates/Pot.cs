using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Pot : Template {

		public override TemplateSettings Settings {
			get {
				return new TemplateSettings () {
					TopBarEnabled = true,
					TopBarColor = Palette.Orange,
					BackgroundColor = Palette.White,
					BackgroundImage = "applause-bg",
					PotEnabled = true,
					CoinsEnabled = true
				};
			}
		}

		List<TextElementUI> instructions;
		List<TextElementUI> Instructions {
			get {
				if (instructions == null) {
					instructions = new List<TextElementUI> ();
					instructions.Add ((TextElementUI)Elements["instruction1"]);
					instructions.Add ((TextElementUI)Elements["instruction2"]);
					instructions.Add ((TextElementUI)Elements["instruction3"]);
					instructions.Add ((TextElementUI)Elements["instruction4"]);
					instructions.Add ((TextElementUI)Elements["instruction5"]);
				}
				return instructions;
			}
		}

		protected override void OnLoadView () {
			// Elements["pot"].Visible = false;
			// Elements["coins"].Visible = false;
			Elements["next"].Visible = false;
			foreach (TextElementUI t in Instructions) {
				t.Visible = false;
			}
		}

		protected override void OnInputEnabled () {
			Co.RepeatAscending (0.5f, 3.5f, Instructions.Count, (int i) => {
				if (i > 0)
					Instructions[i-1].Visible = false;
				Instructions[i].Visible = true;
			}, () => {
				Elements["next"].Visible = true;
			});
		}
	}
}