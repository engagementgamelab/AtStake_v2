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

		Dictionary<string, string> templates = new Dictionary<string, string> () {
			{ "start", "Start" },
			{ "name", "Name" },
			{ "hostjoin", "HostJoin" }
		};

		public void Init (GameInstance gi) {
			this.gi = gi;
			container = ObjectPool.Instantiate<TemplateContainer> ();
			container.transform.SetParent (Transform);
			container.transform.localScale = new Vector3 (0.5f, 0.5f, 1f);
		}

		public void Load (string id, View view) {
			content = ObjectPool.Instantiate (templates[id]).GetComponent<Template> ();
			content.Parent = container.contentContainer;
			content.Transform.localScale = Vector3.one;
			content.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
			content.GetComponent<RectTransform> ().sizeDelta = Vector2.one;
			container.LoadElements (view.Elements);
			container.LoadSettings (content.Settings);
			content.LoadElements (view.Elements);
		}

		public void Unload () {
			ObjectPool.Destroy (content.name.Replace("(Clone)", ""));
		}

		public void AddElement (string key, ScreenElement element) {
			// RenderElements (new Dictionary<string, ScreenElement> () { { key, element } });
		}

		public void RemoveElement (ScreenElement element) {

		}

		void Update () {
			if (gi.Manager != null) 
				Name.text = gi.Name;
		}
	}
}