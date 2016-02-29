using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class TemplateManager : UIElement {

		Template content;

		public TemplatesContainer templatesContainer;
		public TemplateContainer container1;
		public TemplateContainer container2;

		TemplateContainer activeContainer;

		// Template cache
		Dictionary<string, Template> templateLookup = new Dictionary<string, Template> ();

		public void Init () {
			activeContainer = container1;
		}

		public void Load (string id, View view) {
			content = GetTemplateById (id);
			content.gameObject.SetActive (true);
			activeContainer.LoadView (view, content);
			content.LoadView (view);
		}

		public void Unload () {
			content.UnloadView ();
			content.gameObject.SetActive (false);
		}

		Template GetTemplateById (string id) {
			id = id.Replace ("_", "");
			Template template;
			if (templateLookup.TryGetValue (id, out template)) {
				return template;
			} else {
				template = System.Array.Find (activeContainer.templates, x => x.name.ToLower () == id);
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