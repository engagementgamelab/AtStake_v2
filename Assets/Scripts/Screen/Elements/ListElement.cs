using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListElement<T> : ScreenElement where T : ScreenElement {

	public delegate void OnAdd (string id, T t);
	public delegate void OnRemove (string id);

	protected Dictionary<string, T> elements;
	public Dictionary<string, T> Elements {
		get {
			if (elements == null)
				elements = new Dictionary<string, T> ();
			return elements;
		}
	}

	public int Count {
		get { return Elements.Count; }
	}

	public OnAdd onAdd;
	public OnRemove onRemove;

	public ListElement (Dictionary<string, T> elements=null) {
		this.elements = elements;
	}

	public virtual void Add (string id, T t) {
		Elements.Add (id, t);
		if (onAdd != null)
			onAdd (id, t);
		SendUpdateMessage ();
	}

	public void Remove (string id) {
		Elements.Remove (id);
		if (onRemove != null)
			onRemove (id);
		SendUpdateMessage ();
	}

	public virtual void Set (Dictionary<string, T> newElements) {
		Dictionary<string, T> tempElements = new Dictionary<string, T> (Elements);
		foreach (var element in tempElements) {
			if (!newElements.ContainsKey (element.Key)) {
				Remove (element.Key);
			}
		}
		foreach (var element in newElements) {
			if (!elements.ContainsKey (element.Key)) {
				Add (element.Key, element.Value);
			}
		}
	}
}