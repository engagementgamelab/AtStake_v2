using UnityEngine;
using System.Collections;

public abstract class ScreenElementUI : UIElement {
	public string id;
	public abstract void Load (ScreenElement e);
	public abstract void Unload ();
}

public abstract class ScreenElementUI<T> : ScreenElementUI where T : ScreenElement {
	
	T element;

	public override void Load (ScreenElement element) {
		this.element = (T)element;
		ApplyElement (this.element);
	}

	public override void Unload () {
		RemoveElement (element);
		this.element = null;
	}

	public abstract void ApplyElement (T element);
	public virtual void RemoveElement (T element) {}
}
