using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class FinalScoreboard : Scoreboard {

		protected override ScreenElementUI ContinueButton {
			get { return Elements["menu"]; }
		}

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("bottom_button|menu") {
				TopBarColor = Palette.Orange,
				TopBarHeight = TemplateSettings.ShortBar
			};
		}

		protected override void OnLoadView () {

			AvatarElementUI avatar = GetElement<AvatarElementUI> ("winning_player");
			avatar.playerName.ApplyStyle (new TextStyle () {
				FontColor = Palette.Grey,
				FontSize = 32,
				TextAnchor = TextAnchor.MiddleRight
			});
			avatar.playerScore.ApplyStyle (new TextStyle () {
				FontColor = Palette.Avatar.GetColor (avatar.Element.Color),
				FontSize = 32,
				TextAnchor = TextAnchor.MiddleLeft,
				FontStyle = FontStyle.BoldAndItalic
			});

			base.OnLoadView ();
		}

		protected override void OnInputEnabled () {

			List<AvatarInlineElementUI> childElements = SortPlayers ();
			int counter = 0;

			Co.InvokeWhileTrue (2f, 0.25f, () => { return counter < childElements.Count; }, () => {
				childElements[counter].Animate (new UIAnimator.FadeIn (0.75f));
				counter ++;
			}, () => {
				Co.WaitForSeconds (0.75f, () => {
					ContinueButton.Visible = true;
					ContinueButton.Animate (new UIAnimator.FadeIn (0.5f));
				});
			});
		}
	}
}