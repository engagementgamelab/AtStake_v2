using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadioListElement : ListElement<ButtonElement> {

	string selected = "";
	ButtonElement confirm;

	public RadioListElement (string confirmText, System.Action<string> onConfirm, List<string> buttonNames=null) : base (null) {

		confirm = new ButtonElement (confirmText, () => {
			onConfirm (selected);
		}, "confirm") { 
			Interactable = false 
		};

		Elements.Add ("confirm", confirm);

		if (buttonNames != null)
			Set (buttonNames);
	}

	protected override void OnInit () {
		foreach (var b in Elements)
			b.Value.Init (Behaviour);
	}

	public void Add (string name) {
		ButtonElement b = new ButtonElement (name, SelectButton, "select");
		b.Init (Behaviour);
		Add (name, b);
	}

	public void Set (List<string> newNames) {

		Dictionary<string, ButtonElement> newElements = new Dictionary<string, ButtonElement> ();

		foreach (string name in newNames) {
			ButtonElement b = new ButtonElement (name, SelectButton, "select");
			b.Init (Behaviour);
			newElements.Add (name, b);
		}
		newElements.Add ("confirm", confirm);

		Set (newElements);

		// If the selected element was removed, unselect it
		if (!Elements.ContainsKey (selected)) {
			selected = "";
			Elements["confirm"].Interactable = false;
		}
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
