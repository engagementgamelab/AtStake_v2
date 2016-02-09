using UnityEngine;
using System.Collections;

public abstract class ScreenElement {
	public abstract Transform Render (GameScreen screen);
	public abstract void Remove ();
}

public class ScreenElement<T> : ScreenElement where T : UIElement {

	protected T uiElement;
	protected GameScreen screen;

	public override Transform Render (GameScreen screen) {
		this.screen = screen;
		uiElement = ObjectPool.Instantiate<T> ();
		OnRender (uiElement);
		return uiElement.Transform;	
	}

	protected virtual void OnRender (T t) {}

	public override void Remove () {
		try {
			OnRemove (uiElement);
			ObjectPool.Destroy (uiElement);
		} catch {
			throw new System.Exception ("The UIElement has not been set for " + this);
		}
	}

	protected virtual void OnRemove (T t) {}
}