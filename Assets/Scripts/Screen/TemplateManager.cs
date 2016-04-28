using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	/// <summary>
	/// Holds the template containers and loads templates
	/// </summary>
	public class TemplateManager : GameInstanceBehaviour {

		public TemplatesContainer templatesContainer;
		public DebugInfoContainer debug;

		/// <summary>
		/// Create the TemplateManager and initialize it
		/// </summary>
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

		/// <summary>
		/// Load the template with the id and apply the view
		/// </summary>
		public void Load (string id, View view) {
			templatesContainer.Load (id, view);
		}

		void OnEnable () {

			transform.localScale = 			
			#if UNITY_EDITOR && SINGLE_SCREEN
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

			#if UNITY_EDITOR
			if (Parent == null)
				gameObject.SetActive (false);
			#endif
		}
	}
}