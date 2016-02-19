using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TemplateContent : MB {

	public abstract TemplateSettings Settings { get; }

	List<ScreenElementUI> elements;
	List<ScreenElementUI> Elements {
		get {
			if (elements == null) {
				elements = Transform.GetAllChildren ()
					.FindAll (x => x.GetComponent<ScreenElementUI> () != null)
					.ConvertAll (x => x.GetComponent<ScreenElementUI> ());
			}
			return elements;
		}
	}

	ScreenElementUI FindScreenElementById (string id) {
		return Elements.Find (x => x.id == id);
	}

	public void LoadElements (Dictionary<string, ScreenElement> elements) {
		foreach (var element in elements) {
			string k = element.Key;
			if (k == "back" || k == "pot" || k == "coin")
				continue;
			ScreenElementUI e = FindScreenElementById (k);
			if (e != null) e.Init (element.Value);	
		}
	}
}

public struct TemplateSettings {
	public bool TopBarEnabled { get; set; }
	public Color TopBarColor { get; set; }
	public Color BackgroundColor { get; set; }
	public string BackgroundImage { get; set; }
}
