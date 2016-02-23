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

		public RectTransform pot;
		public RectTransform coins;

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

		int PotValue {
			set { pot.GetChild (0).GetComponent<Text> ().text = value.ToString (); }
		}

		int CoinsValue {
			set { coins.GetChild (0).GetComponent<Text> ().text = value.ToString (); }
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
			ScreenElement back;
			if (elements.TryGetValue ("back", out back)) {
				backButton.gameObject.SetActive (true);
				backButton.Load ((BackButtonElement)back);
			} else {
				backButton.gameObject.SetActive (false);
			}

			// Pot
			ScreenElement potEl;
			if (elements.TryGetValue ("pot", out potEl)) {
				pot.gameObject.SetActive (true);
			} else {
				pot.gameObject.SetActive (false);
			}

			// Coins
			ScreenElement coinsEl;
			if (elements.TryGetValue ("coins", out coinsEl)) {
				coins.gameObject.SetActive (true);
			} else {
				coins.gameObject.SetActive (false);
			}
		}

		void LoadSettings (TemplateSettings settings) {
			SetBackground (settings.BackgroundColor, settings.BackgroundImage);
			SetTopBar (settings.TopBarEnabled, settings.TopBarColor);
		}
	}
}