using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class TemplateManager : UIElement {

		public TemplatesContainer templatesContainer;

		public void Load (string id, View view) {
			templatesContainer.Load (id, view);
		}

		void OnEnable () {
			transform.localScale = 			
			#if SINGLE_SCREEN
				new Vector3 (0.5f, 0.5f, 0.5f);
			#else
				Vector3.one;
			#endif
		}
	}
}