using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class TemplateManager : UIElement {

		public TemplatesContainer templatesContainer;

		public void Load (string id, View view) {
			// activeContainer.LoadView (id, view);
			templatesContainer.Load (id, view);
		}

		public void Unload () {
			// activeContainer.UnloadView ();
		}
	}
}