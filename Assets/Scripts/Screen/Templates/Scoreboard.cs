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
				t.Alpha = 0f;
		}

		protected override void OnInputEnabled () {

			List<AvatarInlineElementUI> childElements = SortPlayers ();
			int counter = childElements.Count-1;

			Co.InvokeWhileTrue (0.5f, 2f, () => { return counter >= 0; }, () => {
				childElements[counter].Animate (new UIAnimator.FadeIn (0.75f));
				counter --;
			}, () => {
				Co.WaitForSeconds (0.75f, () => {
					ContinueButton.Visible = true;
					ContinueButton.Animate (new UIAnimator.FadeIn (0.5f));
				});
				TextElementUI instructions;
				if (TryGetElement<TextElementUI> ("decider_instructions", out instructions)) {
					instructions.Visible = true;
					instructions.Animate (new UIAnimator.FadeIn (1f));
				}
			});
		}

		protected List<AvatarInlineElementUI> SortPlayers () {

			List<AvatarInlineElementUI> childElements = Scores.ChildElements;
			childElements.Sort ((x, y) => ValueFromText (x.coinCount).CompareTo (ValueFromText (y.coinCount)));
			childElements.Reverse ();

			for (int i = 0; i < childElements.Count; i ++)
				childElements[i].Transform.SetSiblingIndex (i);

			return childElements;
		}

		int ValueFromText (Text text) {
			return int.Parse (Regex.Match (text.text, @"\d+").Value);
		}
	}
}