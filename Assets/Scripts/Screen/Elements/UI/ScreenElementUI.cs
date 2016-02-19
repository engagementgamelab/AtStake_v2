using UnityEngine;
using System.Collections;

public abstract class ScreenElementUI : UIElement {
	public string id;
	public abstract void Init (ScreenElement e);
	public abstract void Remove ();
}

public abstract class ScreenElementUI<T> : ScreenElementUI where T : ScreenElement {
	
	T element;

	public override void Init (ScreenElement element) {
		this.element = (T)element;
		ApplyElement (this.element);
	}

	public override void Remove () {
		RemoveElement (element);
		this.element = null;
	}

	public abstract void ApplyElement (T element);
	public virtual void RemoveElement (T element) {}
}
