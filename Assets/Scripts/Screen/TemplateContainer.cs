using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class TemplateContainer : MonoBehaviour {

		public Image backgroundColor;
		public Image backgroundImage;
		public Image topBar;

		public BackButtonElementUI backButton;

		public PotElementUI pot;
		public CoinsElementUI coins;

		public RectTransform contentContainer;
		public RectTransform contentArea;

		public Template[] templates;

		Color BackgroundColor {
			set { backgroundColor.color = value; }
		}

		Sprite BackgroundImage {
			set { backgroundImage.sprite = value; }
		}

		Color TopBarColor {
			set { topBar.color = value; }
		}

		public void LoadView (View view, Template template) {
			LoadElements (view.Elements);
			LoadSettings (template.Settings);
		}

		void SetBackground (Color bgColor, string bgImage) {
			BackgroundColor = bgColor;
			BackgroundImage = AssetLoader.LoadBackground (bgImage);
		}

		void SetTopBar (bool enabled, Color topBarColor=new Color()) {
			topBar.gameObject.SetActive (enabled);
			TopBarColor = topBarColor;
		}

		void LoadElements (Dictionary<string, ScreenElement> elements) {

			// Back button
			if (backButton.Loaded)
				backButton.Unload ();

			ScreenElement back;
			if (elements.TryGetValue ("back", out back)) {
				backButton.gameObject.SetActive (true);
				backButton.Load ((BackButtonElement)back);
			} else {
				backButton.gameObject.SetActive (false);
			}

			// Pot
			if (pot.Loaded)
				pot.Unload ();
				
			ScreenElement potEl;
			if (elements.TryGetValue ("pot", out potEl)) {
				pot.Load (potEl);
			}

			// Coins
			if (coins.Loaded)
				coins.Unload ();

			ScreenElement coinsEl;
			if (elements.TryGetValue ("coins", out coinsEl)) {
				coins.Load (coinsEl);
			}
		}

		void LoadSettings (TemplateSettings settings) {
			SetBackground (settings.BackgroundColor, settings.BackgroundImage);
			SetTopBar (settings.TopBarEnabled, settings.TopBarColor);
			pot.gameObject.SetActive (settings.PotEnabled);
			coins.gameObject.SetActive (settings.CoinsEnabled);
		}
	}
}