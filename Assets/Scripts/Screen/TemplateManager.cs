using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class TemplateManager : GameInstanceBehaviour {

		public TemplatesContainer templatesContainer;
		public DebugInfoContainer debug;

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

		void Start () {
			debug.gameObject.SetActive (
			#if SHOW_DEBUG_INFO
				true);
			debug.Init (Game);
			#else
				false);
			#endif
		}
	}
}