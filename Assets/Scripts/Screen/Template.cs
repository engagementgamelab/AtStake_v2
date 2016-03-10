﻿using UnityEngine;
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
					foreach (ScreenElementUI e in childElements) {
						elements.Add (e.id, e);
					}
				}
				return elements;
			}
		}

		Dictionary<string, ScreenElementUI> LoadedElements {
			get { return Elements.Where (x => x.Value.Loaded).ToDictionary (x => x.Key, x => x.Value); }
		}

		protected bool Loaded { get; private set; }

		void OnEnable () { Loaded = false; }

		public void LoadView (View view) {
			LoadElements (view.Elements);
			OnLoadView ();
			Loaded = true;
		}

		public void UnloadView () {
			foreach (var element in LoadedElements) {
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
				ScreenElement elementData;
				if (data.TryGetValue (k, out elementData)) {
					element.Value.Load (elementData);
				} else {
					element.Value.gameObject.SetActive (false);
				}
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

		protected virtual void OnLoadView () {}
		protected virtual void OnUnloadView () {}
		protected virtual void OnInputEnabled () {}
	}

	public struct TemplateSettings {
		public bool TopBarEnabled { get; set; }
		public Color TopBarColor { get; set; }
		public Color BackgroundColor { get; set; }
		public string BackgroundImage { get; set; }
		public bool PotEnabled { get; set; }
		public bool CoinsEnabled { get; set; }
	}
}