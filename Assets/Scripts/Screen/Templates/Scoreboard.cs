using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Templates {

	public class Scoreboard : Template {

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

		ListElementUI<TextElementUI, TextElement> scores;
		ListElementUI<TextElementUI, TextElement> Scores {
			get {
				if (scores == null)
					scores = GetElement<ListElementUI<TextElementUI, TextElement>> ("score_list");
				return scores;
			}
		}

		protected override void OnLoadView () {
			// Scores.Visible = false;
		}

		protected override void OnInputEnabled () {
			// TODO: not quite working
			List<TextElementUI> childElements = Scores.ChildElements;
			childElements.Sort ((x, y) => ValueFromText (x.Text).CompareTo (ValueFromText (y.Text)));
			for (int i = 0; i < childElements.Count; i ++) {
				childElements[0].Transform.SetSiblingIndex (i);
			}
		}

		int ValueFromText (Text text) {
			return int.Parse (Regex.Match (text.text, @"\d+").Value);
		}
	}
}