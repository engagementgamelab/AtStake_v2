using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericList : UIElement {

	public string Id { get; set; }

	public void Add<T> (T element) where T : UIElement {
		element.Transform.SetParent (Transform);
		element.Transform.localScale = Vector3.one;
		element.Transform.localPosition = Vector3.zero;
	}

	public void AddText (string s, bool bold=false) {
		GenericText text = ObjectPool.Instantiate<GenericText> ();
		text.Text.text = s;
		text.Style = bold ? FontStyle.Bold : FontStyle.Normal;
		Add<GenericText> (text);
	}

	public void AddButton (string label, System.Action onPress) {
		GenericButton button = ObjectPool.Instantiate<GenericButton> ();
		button.Init (label, onPress);
		Add<GenericButton> (button);
	}

	public void Remove<T> (T element) where T : UIElement {
		ObjectPool.Destroy<T> (element);
	}

	public void Clear<T> () where T : UIElement {
		ObjectPool.DestroyChildren<T> (Transform);
	}
}
