using UnityEngine;
using System.Collections;

public abstract class ScreenElement {
	public abstract bool Active { get; }
	public abstract UIElement Element { get; }
	public abstract Transform Render (GameScreen screen);
	public abstract void Remove ();
}

public class ScreenElement<T> : ScreenElement where T : UIElement {

	protected T uiElement;
	protected GameScreen screen;

	public override UIElement Element {
		get { return uiElement as UIElement; }
	}

	bool active = false;
	public override bool Active {
		get { return active; }
	}

	public override Transform Render (GameScreen screen) {
		this.screen = screen;
		uiElement = ObjectPool.Instantiate<T> ();
		OnRender (uiElement);
		active = true;
		return uiElement.Transform;	
	}

	protected virtual void OnRender (T t) {}

	public override void Remove () {
		try {
			OnRemove (uiElement);
			ObjectPool.Destroy (uiElement);
			active = false;
		} catch {
			throw new System.Exception ("The UIElement has not been set for " + this);
		}
	}

	protected virtual void OnRemove (T t) {}
}