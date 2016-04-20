using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class AgendaItemReject : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "lt_coins_and_pot") {
				TopBarColor = Palette.Orange,
				TopBarHeight = TemplateSettings.ShortBar,
				BottomBarHeight = TemplateSettings.MediumBar,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true
			};
		}

		AgendaItemResultData data;

		protected override void OnLoadView () {
			data = GetViewData<AgendaItemResultData> ();
		}

		protected override void OnInputEnabled () {

			// Introduce the coin
			AnimElementUI coin = CreateAnimation ();
			coin.SpriteName = "coin";
			coin.Text = "+" + data.CoinCount.ToString ();
			coin.Size = new Vector2 (50, 50);
			coin.LocalPosition = new Vector3 (-50, 25, 0);
			coin.Animate (new UIAnimator.Expand (0.5f));
			
			Co.WaitForSeconds (1f, () => {

				// Introduce the avatar
				Vector3 deciderPosition = new Vector3 (50, 25f, 0);
				AnimElementUI decider = CreateAnimation ();
				decider.AvatarName = data.PlayerAvatarColor;
				decider.Size = new Vector2 (75, 75);
				decider.LocalPosition = deciderPosition;
				decider.Animate (new UIAnimator.Expand (0.5f));

				Co.WaitForSeconds (0.5f, () => {
					coin.Animate (new UIAnimator.Shrink (0.5f));

					Co.WaitForSeconds (0.5f, () => {
						decider.Animate (new UIAnimator.Spin (0.5f, () => {
							decider.Animate (new UIAnimator.Shrink (0.5f));
						}));
					});
				});
			});
		}

		protected override void OnUnloadView () {
			AnimationContainer.Reset ();
		}
	}
}