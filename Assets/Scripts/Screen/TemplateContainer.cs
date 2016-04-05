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
		public Image bottomBar;

		public BackButtonElementUI backButton;
		public PotElementUI pot;
		public CoinsElementUI coins;

		public Template[] templates;

		Template content;
		Dictionary<string, Template> templateLookup = new Dictionary<string, Template> ();

		Dictionary<string, ScreenElementUI> overlayElements;
		Dictionary<string, ScreenElementUI> OverlayElements {
			get {
				if (overlayElements == null) {
					overlayElements = new Dictionary<string, ScreenElementUI> ();
					overlayElements.Add ("back", backButton);
					overlayElements.Add ("pot", pot);
					overlayElements.Add ("coins", coins);
				}
				return overlayElements;
			}
		}

		Color BackgroundColor {
			set { backgroundColor.color = value; }
		}

		Sprite BackgroundImage {
			set { backgroundImage.sprite = value; }
		}

		Color TopBarColor {
			set { topBar.color = value; }
		}

		Color BottomBarColor {
			set { bottomBar.color = value; }
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

			// All templates should be deactivated to start
			foreach (Template template in c.templates)
				template.gameObject.SetActive (false);

			return c;
		}

		public void LoadView (string id, View view) {
			content = GetTemplateById (id);
			content.gameObject.SetActive (true);
			ApplySettings (content.Settings);
			content.LoadView (view, OverlayElements);
		}

		public void UnloadView () {
			content.UnloadView ();
			foreach (Template template in templates)
				template.gameObject.SetActive (false);
		}

		public bool TemplateIsBefore (string template1, string template2) {
			return System.Array.IndexOf (templates, GetTemplateById (template1)) 
				< System.Array.IndexOf (templates, GetTemplateById (template2));

		}

		public void SetInputEnabled () {
			content.InputEnabled ();
		}

		void ApplySettings (TemplateSettings settings) {
			SetBackground (settings.BackgroundColor, settings.BackgroundImage);
			SetTopBar (settings.TopBarHeight, settings.TopBarColor);
			SetBottomBar (settings.BottomBarHeight, settings.BottomBarColor);
		}

		void SetBackground (Color bgColor, string bgImage) {
			BackgroundColor = bgColor;
			
			bool hasBg = !string.IsNullOrEmpty (bgImage);
			backgroundImage.gameObject.SetActive (hasBg);
			if (hasBg)
				BackgroundImage = AssetLoader.LoadBackground (bgImage);
		}

		void SetTopBar (float height, Color topBarColor=new Color()) {

			bool active = height > 0;
			topBar.gameObject.SetActive (active);

			if (active) {
				topBar.GetComponent<LayoutElement> ().preferredHeight = height;
				TopBarColor = topBarColor;
				topBar.transform.GetChild (0).gameObject.SetActive (height > TemplateSettings.ShortBar);
			}
		}

		void SetBottomBar (float height, Color bottomBarColor=new Color()) {
			bottomBar.gameObject.SetActive (height > 0);
			bottomBar.GetComponent<LayoutElement> ().preferredHeight = height;
			BottomBarColor = bottomBarColor;
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