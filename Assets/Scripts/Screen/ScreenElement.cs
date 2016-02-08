using UnityEngine;
using System.Collections;

public abstract class ScreenElement {
	public abstract Transform Render ();
	public abstract void Remove ();
}

public class ScreenElement<T> : ScreenElement where T : UIElement {

	protected UIElement uiElement;

	public override Transform Render () {
		T t = ObjectPool.Instantiate<T> ();
		uiElement = t as UIElement;
		OnRender (t);
		return t.Transform;	
	}

	protected virtual void OnRender (T t) {}

	public override void Remove () {
		try {
			ObjectPool.Destroy (uiElement);
		} catch {
			throw new System.Exception ("The UIElement has not been set for " + this);
		}
	}
}