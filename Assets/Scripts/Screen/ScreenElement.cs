using UnityEngine;
using System.Collections;
using Views;

public abstract class ScreenElement : GameInstanceComponent {

	public abstract bool Active { get; }
	public abstract UIElement Element { get; }
	public abstract Transform Render (Transform parent);
	public abstract void Remove ();
	protected View view; // TODO: this reference might be unused

	public void Init (GameInstanceBehaviour behaviour, View view) {
		base.Init (behaviour);
		this.view = view;
	}
}

public class ScreenElement<T> : ScreenElement where T : UIElement {

	protected T uiElement;
	protected View view;

	public override UIElement Element {
		get { return uiElement as UIElement; }
	}

	bool active = false;
	public override bool Active {
		get { return active; }
	}

	public override Transform Render (Transform parent) {
		uiElement = ObjectPool.Instantiate<T> ();
		OnRender (uiElement);
		uiElement.Parent = parent;
		uiElement.Transform.localScale = Vector3.one;
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