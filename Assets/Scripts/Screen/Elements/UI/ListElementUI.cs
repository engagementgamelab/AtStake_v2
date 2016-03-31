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

	TextStyle style = TextStyle.Paragraph;
	public override TextStyle Style {
		get { return style; }
		set {
			style = value;
			foreach (T child in ChildElements)
				child.Style = style;
		}
	}

	Color listColor = Palette.White;
	public override Color Color {
		get { return listColor; }
		set {
			listColor = value;
			foreach (T child in ChildElements)
				child.Color = listColor;
		}
	}

	public override void ApplyElement (ListElement<U> e) {

		// Load initial elements (if any)
		foreach (var element in e.Elements)
			AddElement (element.Key, element.Value);

		// Listen for elements being added/removed
		e.onAdd += AddElement;
		e.onRemove += RemoveListElement;
	}

	public override void RemoveElement (ListElement<U> e) {
		e.onAdd -= AddElement;
		e.onRemove -= RemoveListElement;
		ObjectPool.DestroyChildren<T> (RectTransform, (T t) => { t.Unload (); });
	}

	void AddElement (string id, U element) {

		// Create a new child element and apply styling

		T t = ObjectPool.Instantiate<T> ();
		t.id = id;
		t.Load (element, Settings);
		t.Parent = RectTransform;
		t.RectTransform.localScale = Vector3.one;

		// All elements in the list will be styled with the default styling unless explicitely overriden

		TextStyle overrideStyle;
		if (Settings.TextStyles.TryGetValue (t.id, out overrideStyle)) {
			t.Style = overrideStyle;
		} else {
			t.Style = Style;
		}

		Color overrideColor;
		if (Settings.Colors.TryGetValue (t.id, out overrideColor)) {
			t.Color = overrideColor;
		} else {
			t.Color = Color;
		}

		OnUpdateListElements ();
	}

	void RemoveListElement (string id) {
		T t = GetChildElement (id);
		t.Unload ();
		ObjectPool.Destroy<T> (t);
		OnUpdateListElements ();
	}

	protected T GetChildElement (string id) {
		return ChildElements.Find (x => x.id == id);
	}

	protected virtual void OnUpdateListElements () {}
}
