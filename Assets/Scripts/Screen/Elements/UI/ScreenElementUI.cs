﻿using UnityEngine;
using System.Collections;

public abstract class ScreenElementUI : UIElement {

	public delegate void OnActiveStateUpdated (bool active);

	public OnActiveStateUpdated onActiveStateUpdated;
	public string id;

	TextStyle style = TextStyle.Paragraph;
	new public virtual TextStyle Style {
		get { return style; }
		set {
			style = value;
			if (Text != null)
				Text.ApplyStyle (style);
		}
	}

	UIAnimator anim;
	protected UIAnimator Anim {
		get {
			if (anim == null) {
				anim = UIAnimator.AttachTo (gameObject); 
				anim.AllowSimultaneous = true;
			}
			return anim;
		}
	}

	public abstract bool Loaded { get; }
	public abstract bool Visible { get; set; }
	protected abstract TemplateSettings Settings { get; }

	public abstract void Load (ScreenElement e, TemplateSettings settings);
	public abstract void Unload ();
	public abstract void InputEnabled ();
	public abstract void PlayAudio ();

	public void Animate (UIAnimator.UIAnimation animation, RectTransform rect=null) {
		Anim.Animate (animation, rect);
	}
}

public abstract class ScreenElementUI<T> : ScreenElementUI where T : ScreenElement {
	
	T element;

	public T Element { get { return element; } }

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

	TemplateSettings settings;
	protected override TemplateSettings Settings {
		get { return settings; }
	}

	public override void Load (ScreenElement element, TemplateSettings settings) {
		this.settings = settings;
		this.element = (T)element;
		Transform.SetLocalScale (1f);
		ApplyElement (this.element);
		element.onUpdate += OnUpdate;
		SetActiveState (element.Active);
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

	public override void PlayAudio () {
		Element.PlayAudio ();
	}

	void OnUpdate (ScreenElement element) {
		SetActiveState (element.Active);
		OnUpdate ((T)element);
	}

	void SetActiveState (bool active) {
		activeState = active;
		gameObject.SetActive (visible && activeState);
		OnSetActive (gameObject.activeSelf);
		if (onActiveStateUpdated != null)
			onActiveStateUpdated (active);
	}

	public abstract void ApplyElement (T element);
	public virtual void RemoveElement (T element) {}
	protected virtual void OnUpdate (T element) {}
	protected virtual void OnInputEnabled (T element) {}
	protected virtual void OnSetActive (bool active) {}
}
