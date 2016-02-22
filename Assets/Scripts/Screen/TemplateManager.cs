using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class TemplateManager : UIElement {

		Text nameText;
		Text Name {
			get {
				if (nameText == null) {
					nameText = GetChildComponent<Text> (0);
				}
				return nameText;
			}
		}

		GameInstance gi;
		TemplateContainer container;
		Template content;

		Dictionary<string, Template> templateLookup = new Dictionary<string, Template> ();

		public void Init (GameInstance gi) {
			this.gi = gi;
			container = ObjectPool.Instantiate<TemplateContainer> ();
			container.transform.SetParent (Transform);
			container.transform.localScale = new Vector3 (0.5f, 0.5f, 1f);
		}

		public void Load (string id, View view) {
			content = GetTemplateById (id);
			content.gameObject.SetActive (true);
			container.LoadElements (view.Elements);
			container.LoadSettings (content.Settings);
			content.LoadElements (view.Elements);
		}

		public void Unload () {
			content.UnloadElements ();
			content.gameObject.SetActive (false);
		}

		public void AddElement (string key, ScreenElement element) {
			// RenderElements (new Dictionary<string, ScreenElement> () { { key, element } });
		}

		public void RemoveElement (ScreenElement element) {

		}

		Template GetTemplateById (string id) {
			Template template;
			if (templateLookup.TryGetValue (id, out template)) {
				return template;
			} else {
				template = System.Array.Find (container.templates, x => x.name.ToLower () == id);
				if (template != null) {
					templateLookup.Add (id, template);
					return template;
				} else {
					throw new System.Exception ("No template for the id '" + id + "' exists.");
				}
			}
		}

		void Update () {
			/*if (Input.GetKeyDown (KeyCode.Q)) {
				Load ("start", new Views.Start ());
			}
			if (Input.GetKeyDown (KeyCode.W)) {
				Load ("name", new Views.Name ());
			}*/
			if (gi.Manager != null) 
				Name.text = gi.Name;
		}
	}
}