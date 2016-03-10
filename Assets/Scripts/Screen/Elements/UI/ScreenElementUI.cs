using UnityEngine;
using System.Collections;

public abstract class ScreenElementUI : UIElement {
	public string id;
	public abstract bool Loaded { get; }
	public abstract bool Visible { get; set; }
	public abstract void Load (ScreenElement e);
	public abstract void Unload ();
	public abstract void InputEnabled ();
}

public abstract class ScreenElementUI<T> : ScreenElementUI where T : ScreenElement {
	
	T element;

	public override bool Loaded { get { return element != null; } }

	// Set by the view (via the data in ScreenElement)
	bool activeState = false;

	// Set by the template
	bool visible = true;
	public override bool Visible {
		get { return visible; }
		set {
			visible = value;
			gameObject.SetActive (visible && activeState);
		}
	}

	public override void Load (ScreenElement element) {
		this.element = (T)element;
		ApplyElement (this.element);
		element.onUpdate += OnUpdate;
		OnSetActive (element.Active);
	}

	public override void Unload () {
		try {
			element.onUpdate -= OnUpdate;
		} catch (System.NullReferenceException e) {
			throw new System.Exception ("A ScreenElement has not been set for " + this + "\n" + e);
		}
		RemoveElement (element);
		element = null;
		activeState = false;
	}

	public override void InputEnabled () {
		OnInputEnabled (element);
	}

	void OnUpdate (ScreenElement element) {
		OnSetActive (element.Active);
		OnUpdate ((T)element);
	}

	protected virtual void OnSetActive (bool active) {
		activeState = active;
		gameObject.SetActive (visible && activeState);
	}

	public abstract void ApplyElement (T element);
	public virtual void RemoveElement (T element) {}
	protected virtual void OnUpdate (T element) {}
	protected virtual void OnInputEnabled (T element) {}
}
