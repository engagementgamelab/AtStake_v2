using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class TemplateContainer : UIElement {

		public Image backgroundColor;
		public Image backgroundImage;
		public Image topBar;

		public BackButtonElementUI backButton;
		public PotElementUI pot;
		public CoinsElementUI coins;

		public Template[] templates;

		Template content;
		Dictionary<string, Template> templateLookup = new Dictionary<string, Template> ();

		Color BackgroundColor {
			set { backgroundColor.color = value; }
		}

		Sprite BackgroundImage {
			set { backgroundImage.sprite = value; }
		}

		Color TopBarColor {
			set { topBar.color = value; }
		}

		public static TemplateContainer Init (TemplatesContainer myContainer, int siblingIndex) {
			TemplateContainer c = ObjectPool.Instantiate<TemplateContainer> ();
			c.transform.SetParent (myContainer.transform);
			c.transform.SetSiblingIndex (siblingIndex);
			c.RectTransform.localScale = Vector3.one;
			c.RectTransform.anchoredPosition = Vector2.zero;
			c.RectTransform.sizeDelta = Vector2.zero;
			c.backButton = myContainer.backButton;
			c.pot = myContainer.pot;
			c.coins = myContainer.coins;
			return c;
		}

		public void LoadView (string id, View view) {
			content = GetTemplateById (id);
			content.gameObject.SetActive (true);
			LoadElements (view.Elements, content.Settings);
			LoadSettings (content.Settings);
			content.LoadView (view);
		}

		public void UnloadView () {
			content.UnloadView ();
			content.gameObject.SetActive (false);
		}

		public bool TemplateIsBefore (string template1, string template2) {
			return System.Array.IndexOf (templates, GetTemplateById (template1)) 
				< System.Array.IndexOf (templates, GetTemplateById (template2));

		}

		public void SetInputEnabled () {
			content.InputEnabled ();
			if (backButton.gameObject.activeSelf)
				backButton.InputEnabled ();
			if (pot.gameObject.activeSelf)
				pot.InputEnabled ();
			if (coins.gameObject.activeSelf)
				coins.InputEnabled ();
		}

		void SetBackground (Color bgColor, string bgImage) {
			BackgroundColor = bgColor;
			BackgroundImage = AssetLoader.LoadBackground (bgImage);
		}

		void SetTopBar (bool enabled, Color topBarColor=new Color()) {
			topBar.gameObject.SetActive (enabled);
			TopBarColor = topBarColor;
		}

		void LoadElements (Dictionary<string, ScreenElement> elements, TemplateSettings settings) {

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
			if (settings.PotEnabled && elements.TryGetValue ("pot", out potEl)) {
				pot.Load (potEl);
			}

			// Coins
			if (coins.Loaded)
				coins.Unload ();

			ScreenElement coinsEl;
			if (settings.CoinsEnabled && elements.TryGetValue ("coins", out coinsEl)) {
				coins.Load (coinsEl);
			}
		}

		void LoadSettings (TemplateSettings settings) {
			SetBackground (settings.BackgroundColor, settings.BackgroundImage);
			SetTopBar (settings.TopBarEnabled, settings.TopBarColor);
			pot.gameObject.SetActive (settings.PotEnabled);
			coins.gameObject.SetActive (settings.CoinsEnabled);
		}

		Template GetTemplateById (string id) {
			id = id.Replace ("_", "");
			Template template;
			if (templateLookup.TryGetValue (id, out template)) {
				return template;
			} else {
				template = System.Array.Find (templates, x => x.name.ToLower () == id);
				if (template != null) {
					templateLookup.Add (id, template);
					return template;
				} else {
					throw new System.Exception ("No template for the view '" + id + "' exists.");
				}
			}
		}
	}
}