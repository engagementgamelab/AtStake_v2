using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class AgendaItemAccept : Template {

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
			AnimationContainer.RunCoinToAvatarAnimation (data.CoinCount.ToString (), data.PlayerAvatarColor);
		}

		protected override void OnUnloadView () {
			AnimationContainer.Reset ();
		}
	}
}