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

		// Template cache
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
			container.LoadView (view, content);
			content.LoadView (view);
		}

		public void Unload () {
			content.UnloadView ();
			content.gameObject.SetActive (false);
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
					throw new System.Exception ("No template for the view '" + id + "' exists.");
				}
			}
		}

		void Update () {
			if (gi.Manager != null) 
				Name.text = gi.Name;
		}
	}
}