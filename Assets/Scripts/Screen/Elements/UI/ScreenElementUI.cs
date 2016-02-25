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
		element.onUpdate += OnUpdate;
	}

	public override void Unload () {
		try {
			element.onUpdate -= OnUpdate;
		} catch {
			throw new System.Exception ("A ScreenElement has not been set for " + this);
		}
		RemoveElement (element);
		element = null;
	}

	protected virtual void OnUpdate (ScreenElement element) {
		OnSetActive (element.Active);
	}

	protected virtual void OnSetActive (bool active) {
		gameObject.SetActive (active);
	}

	public abstract void ApplyElement (T element);
	public virtual void RemoveElement (T element) {}
}
