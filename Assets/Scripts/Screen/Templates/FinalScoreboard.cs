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

		AvatarElementUI avatar;
		AvatarElementUI Avatar {
			get {
				if (avatar == null)
					avatar = GetElement<AvatarElementUI> ("winning_player");
				return avatar;
			}
		}

		protected override void OnLoadView () {

			// AvatarElementUI avatar = GetElement<AvatarElementUI> ("winning_player");
			Avatar.playerName.ApplyStyle (new TextStyle () {
				FontColor = Palette.Black,
				FontSize = 32,
				TextAnchor = TextAnchor.MiddleRight
			});
			Avatar.playerScore.ApplyStyle (new TextStyle () {
				FontColor = Palette.Avatar.GetColor (Avatar.Element.Color),
				FontSize = 32,
				TextAnchor = TextAnchor.MiddleLeft,
				FontStyle = FontStyle.BoldAndItalic
			});

			Avatar.gameObject.SetActive (false);

			base.OnLoadView ();
		}

		protected override void OnInputEnabled () {

			List<AvatarInlineElementUI> childElements = SortPlayers ();
			int counter = 0;

			Co.WaitForSeconds (1f, () => {

				Avatar.gameObject.SetActive (true);
				Avatar.RandomAnimations = false;

				Co.InvokeWhileTrue (2.5f, 0.33f, () => { return counter < childElements.Count; }, () => {
					childElements[counter].Animate (new UIAnimator.FadeIn (0.75f));
					counter ++;
				}, () => {
					Co.WaitForSeconds (0.75f, () => {
						ContinueButton.Visible = true;
						ContinueButton.Animate (new UIAnimator.FadeIn (0.5f));
					});
				});
			});

		}
	}
}