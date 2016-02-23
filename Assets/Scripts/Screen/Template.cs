using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	/// <summary>
	/// Template takes a view and renders the screen elements.
	/// All ScreenElements in the view should correspond to ScreenElementUIs in the Template
	/// </summary>
	public abstract class Template : MB {

		public abstract TemplateSettings Settings { get; }

		Dictionary<string, ScreenElementUI> elements;
		Dictionary<string, ScreenElementUI> Elements {
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

		public void LoadView (View view) {
			LoadElements (view.Elements);
		}

		public void UnloadView () {
			foreach (var element in Elements) {
				element.Value.Unload ();
			}
		}
		
		void LoadElements (Dictionary<string, ScreenElement> elements) {
			foreach (var element in elements) {
				string k = element.Key;
				if (k == "back" || k == "pot" || k == "coin")
					continue;
				#if UNITY_EDITOR
				try {
				#endif
					Elements[k].Load (element.Value);
				#if UNITY_EDITOR
				} catch (KeyNotFoundException) {
					Debug.LogWarning ("The template '" + this + "' does not contain a screen element with the id '" + k + "' so it will not be rendererd");
				}
				#endif
			}
		}
	}

	public struct TemplateSettings {
		public bool TopBarEnabled { get; set; }
		public Color TopBarColor { get; set; }
		public Color BackgroundColor { get; set; }
		public string BackgroundImage { get; set; }
	}
}