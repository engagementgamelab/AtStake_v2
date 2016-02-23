using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ListElementUI<T, U> : ScreenElementUI<ListElement<U>> where T : ScreenElementUI where U : ScreenElement {

	Dictionary<string, T> Children {
		get {
			Dictionary<string, T> children = new Dictionary<string, T> ();
			foreach (Transform child in RectTransform) {
				T t = child.GetComponent<T> ();
				children.Add (t.id, t);
			}
			return children;
		}
	}

	public override void ApplyElement (ListElement<U> e) {
		e.onAdd += AddElement;
		e.onRemove += RemoveElement;
	}

	public override void RemoveElement (ListElement<U> e) {
		e.onAdd -= AddElement;
		e.onRemove -= RemoveElement;
	}

	void AddElement (string id, U element) {
		T t = ObjectPool.Instantiate<T> ();
		t.id = id;
		t.Load (element);
		t.Parent = RectTransform;
	}

	void RemoveElement (string id) {
		T t = Children[id];
		t.Unload ();
		ObjectPool.Destroy<T> (t);
	}
}
