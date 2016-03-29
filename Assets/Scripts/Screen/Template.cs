using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	/// <summary>
	/// Template takes a view and renders the screen elements.
	/// All ScreenElements in the view should correspond to ScreenElementUIs in the Template
	/// Templates should handle any visual effects (animations, timers, etc.) but not contain any game logic
	/// </summary>
	public abstract class Template : MB {

		public abstract TemplateSettings Settings { get; }

		Dictionary<string, ScreenElementUI> elements;
		protected Dictionary<string, ScreenElementUI> Elements {
			get {
				if (elements == null) {

					List<ScreenElementUI> childElements = Transform.GetAllChildren ()
						.FindAll (x => x.GetComponent<ScreenElementUI> () != null)
						.ConvertAll (x => x.GetComponent<ScreenElementUI> ());

					elements = new Dictionary<string, ScreenElementUI> ();
					foreach (ScreenElementUI e in childElements)
						elements.Add (e.id, e);

					foreach (var e in overlayElements)
						elements.Add (e.Key, e.Value);
				}
				return elements;
			}
		}

		Dictionary<string, ScreenElementUI> LoadedElements {
			get { return Elements.Where (x => x.Value.Loaded).ToDictionary (x => x.Key, x => x.Value); }
		}

		protected bool Loaded { get; private set; }
		Dictionary<string, ScreenElementUI> overlayElements;

		void OnEnable () { Loaded = false; }

		public void LoadView (View view, Dictionary<string, ScreenElementUI> overlayElements) {
			this.overlayElements = overlayElements;
			LoadElements (view.Elements);
			OnLoadView ();
			Loaded = true;
		}

		public void UnloadView () {

			// Unload all the elements
			// Skip overlay elements - they get unloaded just before being loaded
			foreach (var element in LoadedElements) {
				if (!IsOverlayElement (element.Key))
					element.Value.Unload ();
			}
			OnUnloadView ();
			Loaded = false;
		}

		public void InputEnabled () {
			foreach (var element in LoadedElements) {
				element.Value.InputEnabled ();
			}
			OnInputEnabled ();
		}
		
		void LoadElements (Dictionary<string, ScreenElement> data) {

			// Loads the template elements based on the provided data
			// If the template has an element without any associated data, it is not rendered
			foreach (var element in Elements) {

				string k = element.Key;
				ScreenElementUI v = element.Value;
				ScreenElement elementData;

				// Special case: Unload overlay elements on load
				// Then load them if they're active in this view
				if (IsOverlayElement (k)) {
					if (v.Loaded) v.Unload ();
					if (k == "coins")
						v.Visible = Settings.CoinsEnabled;
					if (k == "pot")
						v.Visible = Settings.PotEnabled;
				}

				// Apply content
				if (data.TryGetValue (k, out elementData)) {
					v.Load (elementData);
				} else {
					v.gameObject.SetActive (false); 
				}
			}

			// Apply colors
			if (Settings.Colors != null) {
				foreach (var color in Settings.Colors)
					Elements[color.Key].Color = color.Value;
			}

			// Apply text styles
			if (Settings.TextStyles != null) {
				foreach (var style in Settings.TextStyles)
					((TextElementUI)Elements[style.Key]).Style = style.Value;
			}

			// Throw a warning if there's data without an associated template element (only in editor)
			#if UNITY_EDITOR
			foreach (var element in data) {
				string k = element.Key;
				if (k == "back" || k == "pot" || k == "coins")
					continue;
				if (!Elements.ContainsKey (k)) {
					Debug.LogWarning ("The template '" + this + "' does not contain a screen element with the id '" + k + "' so it will not be rendererd");
				}
			}
			#endif
		}

		protected T GetElement<T> (string id) where T : ScreenElementUI {
			try {
				return (T)LoadedElements[id];
			} catch {
				throw new System.Exception ("The template '" + this + "' does not contain an element with the id '" + id + "'");
			}
		}

		bool IsOverlayElement (string id) {
			return id == "coins" || id == "pot" || id == "back";
		}

		protected virtual void OnLoadView () {}
		protected virtual void OnUnloadView () {}
		protected virtual void OnInputEnabled () {}
	}

	public struct TemplateSettings {

		public const float TallBar = 92;
		public const float ShortBar = 24;

		public float TopBarHeight { get; set; }
		public Color TopBarColor { get; set; }
		public float BottomBarHeight { get; set; }
		public Color BottomBarColor { get; set; }

		public Color BackgroundColor { get; set; }
		public string BackgroundImage { get; set; }

		public bool PotEnabled { get; set; }
		public bool CoinsEnabled { get; set; }

		public Dictionary<string, Color> Colors { get; set; }
		public Dictionary<string, TextStyle> TextStyles { get; set; }
	}
}