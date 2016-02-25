﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListElement<T> : ScreenElement where T : ScreenElement {

	public delegate void OnAdd (string id, T t);
	public delegate void OnRemove (string id);

	Dictionary<string, T> elements;
	public Dictionary<string, T> Elements {
		get {
			if (elements == null)
				elements = new Dictionary<string, T> ();
			return elements;
		}
	}

	public OnAdd onAdd;
	public OnRemove onRemove;

	public ListElement (Dictionary<string, T> elements=null) {
		this.elements = elements;
	}

	public void Add (string id, T t) {
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
}