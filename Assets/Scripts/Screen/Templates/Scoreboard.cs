using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Templates {

	public class Scoreboard : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button") {
				TopBarColor = Palette.Orange,
				TopBarHeight = TemplateSettings.ShortBar,
				BottomBarHeight = TemplateSettings.MediumBar,
				BackgroundColor = Palette.White,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "score_list", TextStyle.Paragraph }
				}
			};
		}
		
		ListElementUI<TextElementUI, TextElement> scores;
		ListElementUI<TextElementUI, TextElement> Scores {
			get {
				if (scores == null)
					scores = GetElement<ListElementUI<TextElementUI, TextElement>> ("score_list");
				return scores;
			}
		}

		protected virtual ScreenElementUI ContinueButton {
			get { return Elements["next"]; }
		}

		protected override void OnLoadView () {

			ContinueButton.Visible = false;

			TextElementUI instructions;
			if (TryGetElement<TextElementUI> ("previous_decider_instructions", out instructions)) {
				instructions.Visible = false;
			}

			foreach (TextElementUI t in Scores.ChildElements)
				t.Visible = false;
		}

		protected override void OnInputEnabled () {

			List<TextElementUI> childElements = Scores.ChildElements;
			childElements.Sort ((x, y) => ValueFromText (x.Text).CompareTo (ValueFromText (y.Text)));
			childElements.Reverse ();

			for (int i = 0; i < childElements.Count; i ++)
				childElements[i].Transform.SetSiblingIndex (i);

			int counter = childElements.Count-1;
			Co.InvokeWhileTrue (0.5f, 2.5f, () => { return counter >= 0; }, () => {
				childElements[counter].Visible = true;
				counter --;
			}, () => {
				ContinueButton.Visible = true;
				TextElementUI instructions;
				if (TryGetElement<TextElementUI> ("previous_decider_instructions", out instructions)) {
					instructions.Visible = true;
				}
			});
		}

		int ValueFromText (Text text) {
			return int.Parse (Regex.Match (text.text, @"\d+").Value);
		}
	}
}