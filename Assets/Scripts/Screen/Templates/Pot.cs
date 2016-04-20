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
		}

		void FadeInElement (string id) {
			if (!Elements[id].Visible) {
				Elements[id].Visible = true;
				Elements[id].Animate (new UIAnimator.FadeIn (0.5f));
			}
		}

		void RunDeciderAnimation () {

			if (Elements["instruction1"].Visible) {

				Co.WaitForSeconds (0.5f, () => {

					// Introduce the coin
					AnimElementUI coin = AnimElementUI.Create (AnimationContainer, Vector3.zero);
					coin.SpriteName = "coin";
					coin.Text = "+" + data.DeciderCoinCount.ToString ();
					coin.Size = new Vector2 (50, 50);
					coin.LocalPosition = new Vector3 (-50, 25, 0);
					coin.Animate (new UIAnimator.Expand (0.5f));

					Co.WaitForSeconds (1f, () => {

						// Introduce the Decider
						Vector3 deciderPosition = new Vector3 (50, 25f, 0);
						AnimElementUI decider = AnimElementUI.Create (AnimationContainer, Vector3.zero);
						decider.AvatarName = data.DeciderAvatarColor;
						decider.Size = new Vector2 (75, 75);
						decider.LocalPosition = deciderPosition;
						decider.Animate (new UIAnimator.Expand (0.5f));

						Co.WaitForSeconds (1f, () => {

							// Move the coin to the Decider and shrink out
							Co.WaitForSeconds (0.5f, () => {
								coin.Animate (new UIAnimator.Shrink (1.5f));
							});

							coin.Animate (new UIAnimator.Move (1f, deciderPosition, () => {
								decider.Animate (new UIAnimator.Shrink (0.5f, () => {
									coin.Destroy ();
									decider.Destroy ();
								}));
							}));
						});
					});
				});
			}
		}

		void RunPlayerAnimation () {
			
		}

		protected override void OnUnloadView () {
			instructions = null;
		}
	}
}