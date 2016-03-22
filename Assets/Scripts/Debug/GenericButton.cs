using UnityEngine;
using System.Collections;

public class GenericButton : UIElement {

	public string Id { get; private set; }

	public void Init (string label, System.Action onPress) {
		Id = label;
		Text.text = label;
		Layout.minWidth = label.Length * 9;
		RemoveButtonListeners ();
		AddButtonListener (onPress);
	}
}
