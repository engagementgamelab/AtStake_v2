﻿using UnityEngine;
using System.Collections;

public abstract class ScreenElementUI : UIElement {
	public string id;
	public abstract bool Loaded { get; }
	public abstract void Load (ScreenElement e);
	public abstract void Unload ();
	public abstract void InputEnabled ();
}

public abstract class ScreenElementUI<T> : ScreenElementUI where T : ScreenElement {
	
	T element;

	public override bool Loaded { get { return element != null; } }

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
	}

	public override void InputEnabled () {
		OnInputEnabled (element);
	}

	void OnUpdate (ScreenElement element) {
		OnSetActive (element.Active);
		OnUpdate ((T)element);
	}

	protected virtual void OnSetActive (bool active) {
		gameObject.SetActive (active);
	}

	public abstract void ApplyElement (T element);
	public virtual void RemoveElement (T element) {}
	protected virtual void OnUpdate (T element) {}
	protected virtual void OnInputEnabled (T element) {}
}
