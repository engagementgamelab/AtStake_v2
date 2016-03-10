using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ListElementUI<T, U> : ScreenElementUI<ListElement<U>> where T : ScreenElementUI where U : ScreenElement {

	public List<T> ChildElements {
		get {
			List<T> childElements = new List<T> ();
			foreach (Transform child in RectTransform)
				childElements.Add (child.GetComponent<T> ());
			return childElements;
		}
	}

	public override void ApplyElement (ListElement<U> e) {

		// Load initial elements (if any)
		foreach (var element in e.Elements)
			AddElement (element.Key, element.Value);

		// Listen for elements being added/removed
		e.onAdd += AddElement;
		e.onRemove += RemoveElement;
	}

	public override void RemoveElement (ListElement<U> e) {
		e.onAdd -= AddElement;
		e.onRemove -= RemoveElement;
		ObjectPool.DestroyChildren<T> (RectTransform, (T t) => { t.Unload (); });
	}

	void AddElement (string id, U element) {
		T t = ObjectPool.Instantiate<T> ();
		t.id = id;
		t.Load (element);
		t.Parent = RectTransform;
		t.RectTransform.localScale = Vector3.one;
	}

	void RemoveElement (string id) {
		T t = ChildElements.Find (x => x.id == id);
		t.Unload ();
		ObjectPool.Destroy<T> (t);
	}
}
