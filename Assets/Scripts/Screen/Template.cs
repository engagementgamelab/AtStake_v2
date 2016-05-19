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

		TemplateSettings settings = null;
		public TemplateSettings Settings {
			get {
				if (settings == null)
					settings = LoadSettings ();
				return settings;
			}
		}

		Dictionary<string, ScreenElementUI> elements;
		protected Dictionary<string, ScreenElementUI> Elements {
			get {
				if (elements == null) {

					List<ScreenElementUI> childElements = new List<ScreenElementUI> ();
					foreach (Transform child in Transform.GetAllChildren ()) {
						ScreenElementUI s = child.GetComponent<ScreenElementUI> ();
						if (s != null && s.id != "")
							childElements.Add (s);
					}

					elements = new Dictionary<string, ScreenElementUI> ();
					foreach (ScreenElementUI e in childElements)
					{
						try {
							elements.Add (e.id, e);
						}
						catch(System.Exception ex) {
							throw new System.Exception("'" + e.id + "' already in elements for this view!");
						}
					}

					foreach (var e in overlayElements)
						elements.Add (e.Key, e.Value);
				}
				return elements;
			}
		}

		public List<string> ElementIds {
			get { return new List<string> (Elements.Keys); }
		}

		protected Dictionary<string, ScreenElementUI> LoadedElements {
			get { return Elements.Where (x => x.Value.Loaded).ToDictionary (x => x.Key, x => x.Value); }
		}

		AnimationContainer animationContainer;
		protected AnimationContainer AnimationContainer {
			get {
				if (animationContainer == null)
					animationContainer = Transform.GetChild<AnimationContainer> ();//.GetComponent<RectTransform> ();
				return animationContainer;
			}
		}

		protected bool Loaded { get; private set; }
		protected UIAnimator anim;
		ViewData data;
		Dictionary<string, ScreenElementUI> overlayElements = new Dictionary<string, ScreenElementUI> ();

		void OnEnable () { 
			anim = UIAnimator.AttachTo (gameObject);
			Loaded = false; 
		}
		
		public void LoadView (View view, Dictionary<string, ScreenElementUI> overlayElements) {
			this.overlayElements = overlayElements;
			data = view.Data;
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
			foreach (var element in LoadedElements)
				element.Value.InputEnabled ();
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
					v.Load (elementData, Settings);
				} else {
					v.gameObject.SetActive (false); 
				}
			}

			// Apply text styles
			if (Settings.TextStyles != null) {
				foreach (var style in Settings.TextStyles) {
					ScreenElementUI se;
					if (Elements.TryGetValue (style.Key, out se)) {
						se.Style = style.Value;
					}
				}
			}

			// Apply colors
			if (Settings.Colors != null) {
				foreach (var color in Settings.Colors) {
					ScreenElementUI se;
					if (Elements.TryGetValue (color.Key, out se)) {
						se.Color = color.Value;
					}
				}
			}

			// Throw a warning if there's data without an associated template element (only in editor)
			#if UNITY_EDITOR
			foreach (var element in data) {
				string k = element.Key;
				if (IsOverlayElement (k))
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
			} catch (KeyNotFoundException e) {
				throw new System.Exception ("The template '" + this + "' does not contain an element with the id '" + id + "'\n" + e);
			}
		}

		protected bool TryGetElement<T> (string id, out T elem) where T : ScreenElementUI {
			ScreenElementUI se;
			if (LoadedElements.TryGetValue (id, out se)) {
				elem = (T)se;
				return true;
			}
			elem = null;
			return false;
		}

		protected T GetViewData<T> () where T : ViewData {
			return (T)data;
		}

		protected AnimElementUI CreateAnimation (Vector3 position=new Vector3()) {
			return AnimElementUI.Create (AnimationContainer.Transform, position);
		}

		bool IsOverlayElement (string id) {
			return id == "coins" || id == "pot" || id == "back" || id == "next";
		}

		protected abstract TemplateSettings LoadSettings ();
		protected virtual void OnLoadView () {}
		protected virtual void OnUnloadView () {}
		protected virtual void OnInputEnabled () {}
	}
}