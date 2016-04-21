using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Templates {

	public class Scoreboard : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("bottom_button|next_round") {
				TopBarColor = Palette.Orange,
				TopBarHeight = TemplateSettings.ShortBar,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "round_end", new TextStyle ()
						{
							FontSize = 28,
							FontStyle = FontStyle.Bold,
							FontColor = Palette.Grey
						}
					}
				}
			};
		}
		
		ListElementUI<AvatarInlineElementUI, AvatarElement> scores;
		ListElementUI<AvatarInlineElementUI, AvatarElement> Scores {
			get {
				if (scores == null)
					scores = GetElement<ListElementUI<AvatarInlineElementUI, AvatarElement>> ("score_list");
				return scores;
			}
		}

		protected virtual ScreenElementUI ContinueButton {
			get { return Elements["next_round"]; }
		}

		protected override void OnLoadView () {

			ContinueButton.Visible = false;

			TextElementUI instructions;
			if (TryGetElement<TextElementUI> ("decider_instructions", out instructions)) {
				instructions.Visible = false;
			}

			foreach (AvatarInlineElementUI t in Scores.ChildElements)
				t.Visible = false;
		}

		protected override void OnInputEnabled () {

			List<AvatarInlineElementUI> childElements = Scores.ChildElements;
			childElements.Sort ((x, y) => ValueFromText (x.coinCount).CompareTo (ValueFromText (y.coinCount)));
			childElements.Reverse ();

			for (int i = 0; i < childElements.Count; i ++)
				childElements[i].Transform.SetSiblingIndex (i);

			int counter = childElements.Count-1;
			Co.InvokeWhileTrue (0.5f, 2.5f, () => { return counter >= 0; }, () => {
				childElements[counter].Visible = true;
				counter --;
			}, () => {
				Co.WaitForSeconds (0.75f, () => {
					ContinueButton.Visible = true;
					ContinueButton.Animate (new UIAnimator.Expand (1f));
				});
				TextElementUI instructions;
				if (TryGetElement<TextElementUI> ("decider_instructions", out instructions)) {
					instructions.Visible = true;
					instructions.Animate (new UIAnimator.Expand (1f));
				}
			});
		}

		int ValueFromText (Text text) {
			return int.Parse (Regex.Match (text.text, @"\d+").Value);
		}
	}
}