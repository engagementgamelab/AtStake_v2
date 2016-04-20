using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class Pot : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("next_button", "coins_and_pot") {
				TopBarHeight = TemplateSettings.ShortBar,
				TopBarColor = Palette.LtTeal,
				BottomBarHeight = TemplateSettings.MediumBar,
				BottomBarColor = Palette.LtTeal,
				BackgroundColor = Palette.White,
				PotEnabled = true,
				CoinsEnabled = true
			};
		}

		List<TextElementUI> instructions;
		List<TextElementUI> Instructions {
			get {
				if (instructions == null) {
					instructions = new List<TextElementUI> ();
					instructions.Add ((TextElementUI)Elements["instruction1"]);
					if (LoadedElements.ContainsKey ("instruction2"))
						instructions.Add ((TextElementUI)Elements["instruction2"]);
					instructions.Add ((TextElementUI)Elements["instruction3"]);
					instructions.Add ((TextElementUI)Elements["instruction4"]);
					instructions.Add ((TextElementUI)Elements["instruction5"]);
				}
				return instructions;
			}
		}

		PotData data;

		protected override void OnLoadView () {
			data = GetViewData<PotData> ();
			Elements["pot"].Visible = false;
			Elements["coins"].Visible = false;
			Elements["next"].Visible = false;
			foreach (TextElementUI t in Instructions) {
				t.Visible = false;
			}
		}

		protected override void OnInputEnabled () {
			Co.RepeatAscending (0.5f, 4f, Instructions.Count, (int i) => {

				// Wait until the view is loaded
				if (!Loaded) return;

				// Remove the previous instruction
				if (i > 0) {
					Instructions[i-1].Animate (new UIAnimator.FadeOut (0.2f, () => {
						Instructions[i-1].Visible = false;
						ShowInstruction (i);
					}));
				} else {
					ShowInstruction (i);
				}
				
			}, () => {
				if (Loaded)
					Elements["next"].Visible = true;
			});
		}

		void ShowInstruction (int index) {

			// Show the current instruction
			Instructions[index].Visible = true;
			Instructions[index].Animate (new UIAnimator.FadeIn (0.5f));

			if (Elements["instruction1"].Visible && Elements["next"].Loaded)
				FadeInElement ("coins");
			if (Elements["instruction2"].Visible)
				FadeInElement ("coins");
			if (Elements["instruction4"].Visible) 
				FadeInElement ("pot");

			RunDeciderAnimation ();
			RunPlayerAnimation ();
			RunWinnerAnimation ();
			RunPotAnimation ();
		}

		void FadeInElement (string id) {
			if (!Elements[id].Visible) {
				Elements[id].Visible = true;
				Elements[id].Animate (new UIAnimator.FadeIn (0.5f));
			}
		}

		void RunDeciderAnimation () {
			if (Elements["instruction1"].Visible)
				RunCoinAnimation (data.DeciderCoinCount.ToString (), data.DeciderAvatarColor);
		}

		void RunPlayerAnimation () {
			if (Elements["instruction2"].Visible)
				RunCoinAnimation (data.PlayerCoinCount.ToString (), data.PlayerAvatarColor);
		}

		void RunWinnerAnimation () {

			if (Elements["instruction3"].Visible) {

				Co.WaitForSeconds (0.5f, () => {
					AnimElementUI trophy = CreateAnimation ();
					trophy.SpriteName = "trophy";
					trophy.Size = new Vector2 (100, 100);
					trophy.Animate (new UIAnimator.Expand (1f));
					trophy.Animate (new UIAnimator.Spin (1f));

					Co.WaitForSeconds (3.5f, () => {
						trophy.Animate (new UIAnimator.Spin (1f));
						trophy.Animate (new UIAnimator.Shrink (1f, () => {
							trophy.Destroy ();
						}));
					});
				});
			}
		}

		void RunPotAnimation () {

			if (Elements["instruction4"].Visible) {

				Co.WaitForSeconds (0.5f, () => {
					AnimElementUI pot = CreateAnimation ();
					pot.SpriteName = "coin_stack";
					pot.Text = data.PotCount.ToString ();
					pot.Size = new Vector2 (100, 100);
					pot.Animate (new UIAnimator.Expand (1.5f));
				});
			}
		}

		void RunCoinAnimation (string coinCount, string avatarColor) {

			Co.WaitForSeconds (0.5f, () => {
				AnimationContainer.RunCoinToAvatarAnimation (coinCount, avatarColor);
			});
		}

		protected override void OnUnloadView () {
			instructions = null;
			AnimationContainer.Reset ();
		}
	}
}