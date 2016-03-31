using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class TemplateManager : GameInstanceBehaviour {

		public TemplatesContainer templatesContainer;
		public DebugInfoContainer debug;

		public static TemplateManager Init (Transform parent) {

			// When using the Template Editor, a "loose" manager will be floating around the scene - this disables it
			TemplateManager[] objs = Object.FindObjectsOfType (typeof (TemplateManager)) as TemplateManager[];
			foreach (TemplateManager m in objs) {
				if (m.Parent == null)
					m.gameObject.SetActive (false);
			}

			TemplateManager manager = ObjectPool.Instantiate<TemplateManager> ();
			manager.Transform.SetParent (parent);
			return manager;
		}

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
			Co.YieldWhileTrue (() => { return Parent == null; }, () => {
				debug.Init (Game);
			});
			#else
				false);
			#endif
		}
	}
}