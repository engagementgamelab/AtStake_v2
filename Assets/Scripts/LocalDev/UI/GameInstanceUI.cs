using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameInstanceUI : UIElement {

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
	ScreenTemplate template;

	public void Init (GameInstance gi) {
		this.gi = gi;
		template = ObjectPool.Instantiate<ScreenTemplate> ();
		template.transform.SetParent (Transform);
		template.transform.localScale = new Vector3 (0.5f, 0.5f, 1f);
	}

	public void RenderElements (Dictionary<string, ScreenElement> elements) {
		foreach (var element in elements) {
			element.Value.Render ().SetParent (template.contentArea);
		}
	}

	public void AddElement (string key, ScreenElement element) {

	}

	public void RemoveElement (string key) {

	}

	public void RemoveElements (Dictionary<string, ScreenElement> elements) {
		foreach (var element in elements) {
			element.Value.Remove ();
		}
	}

	void Update () {
		if (gi.Manager != null) 
			Name.text = gi.Name;
	}
}
