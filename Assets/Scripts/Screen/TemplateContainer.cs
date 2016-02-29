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

		public void LoadView (string id, View view) {
			content = GetTemplateById (id);
			content.gameObject.SetActive (true);
			LoadElements (view.Elements);
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