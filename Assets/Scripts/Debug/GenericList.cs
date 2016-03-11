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

	public void Clear<T> () where T : UIElement {
		ObjectPool.DestroyChildren<T> (Transform);
	}
}
