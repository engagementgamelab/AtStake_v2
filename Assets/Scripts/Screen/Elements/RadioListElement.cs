using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadioListElement : ListElement<ButtonElement> {

	string selected = "";

	public RadioListElement (string confirmText, System.Action<string> onConfirm, List<string> buttonNames=null) : base (null) {

		Elements.Add ("confirm", new ButtonElement (confirmText, () => {
			onConfirm (selected);
		}) { Interactable = false });

		if (buttonNames != null)
			Set (buttonNames);
	}

	public void Add (string name) {
		Add (name, new ButtonElement (name, SelectButton));
	}

	public void Set (List<string> newNames) {
		Dictionary<string, ButtonElement> newElements = new Dictionary<string, ButtonElement> ();
		foreach (string name in newNames)
			newElements.Add (name, new ButtonElement (name, SelectButton));
		newElements.Add ("confirm", Elements["confirm"]);
		Set (newElements);
	}

	void SelectButton (ButtonElement b) {

		// unselect previous
		if (selected != "")
			Elements[selected].Interactable = true;

		// select this
		selected = b.Text;
		b.Interactable = false;
		Elements["confirm"].Interactable = true;
	}
}
