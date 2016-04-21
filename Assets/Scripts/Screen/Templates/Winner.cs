using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class Winner : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "lt_coins_and_pot") {
				TopBarColor = Palette.Orange,
				TopBarHeight = TemplateSettings.ShortBar,
				BottomBarHeight = TemplateSettings.MediumBar,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "winner_name", TextStyle.Header },
					{ "coins_won", TextStyle.Header3 }
				}
			};
		}

		protected override void OnLoadView () {
			Elements["winner_name"].RectTransform.localScale = Vector3.zero;
			Elements["avatar"].RectTransform.localScale = Vector3.zero;
			Elements["coins_won"].RectTransform.localScale = Vector3.zero;
			Elements["next"].Visible = false;
		}

		protected override void OnInputEnabled () {
			Co.WaitForSeconds (1f, () => {

				// Show avatar
				Elements["avatar"].Animate (new UIAnimator.Expand (1f));

				Co.WaitForSeconds (0.5f, () => {
					
					// Show winner's name
					Elements["winner_name"].Animate (new UIAnimator.Expand (1f));

					Co.WaitForSeconds (0.5f, () => {

						// Show winnings
						Elements["coins_won"].Animate (new UIAnimator.Expand (1f, () => {
							Elements["next"].Visible = true;
							Elements["next"].Animate (new UIAnimator.FadeIn (0.5f));
						}));
					});
				});
			});
		}
	}
}