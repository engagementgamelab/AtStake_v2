using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

		public void SetBackground (Color bgColor, string bgImage) {
			BackgroundColor = bgColor;
			BackgroundImage = AssetLoader.LoadBackground (bgImage);
		}

		public void SetTopBar (bool enabled, Color topBarColor=new Color()) {
			topBar.gameObject.SetActive (enabled);
			TopBarColor = topBarColor;
		}

		public void LoadElements (Dictionary<string, ScreenElement> elements) {

			// Back button
			ScreenElement back;
			if (elements.TryGetValue ("back", out back)) {
				backButton.gameObject.SetActive (true);
				backButton.Init ((BackButtonElement)back);
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

		public void LoadSettings (TemplateSettings settings) {
			SetBackground (settings.BackgroundColor, settings.BackgroundImage);
			SetTopBar (settings.TopBarEnabled, settings.TopBarColor);
		}
	}
}