using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameScreen : GameInstanceRef {

	public abstract Dictionary<string, ScreenElement> Elements { get; }
	Dictionary<string, ScreenElement> dynamicElements = new Dictionary<string, ScreenElement> ();

	Transform canvas;
	protected GameScreenManager screens;

	public void Init (GameScreenManager screens, Transform canvas) {
		this.screens = screens;
		this.canvas = canvas;
		Init (screens);
	}
	
	public void Show () {
		Render ();
		OnShow ();
	}

	public void Hide () {
		foreach (var element in Elements) {
			element.Value.Remove ();
		}
		foreach (var element in dynamicElements) {
			element.Value.Remove ();
		}
		dynamicElements.Clear ();
		OnHide ();
	}

	void Render () {
		foreach (var element in Elements) {
			element.Value.Render (this).SetParent (canvas);
		}
	}

	void RenderDynamic () {
		foreach (var element in dynamicElements) {
			ScreenElement s = element.Value;
			if (!s.Active)
				s.Render (this).SetParent (canvas);
		}
	}

	protected T GetScreenElement<T> (string id) where T : ScreenElement {
		return Elements[id] as T;
	}

	protected void AddElement (string id, ScreenElement element) {
		dynamicElements.Add (id, element);
		RenderDynamic ();
	}

	protected void RemoveElement (string id) {
		dynamicElements[id].Remove ();
		dynamicElements.Remove (id);
		RenderDynamic ();
	}

	// Routing
	public void GotoScreen (string id) {
		screens.SetScreen (id);
	}

	public void GoBack () {
		screens.SetScreenPrevious ();
	}

	// Events
	public virtual void OnDisconnect () {}
	protected virtual void OnShow () {}
	protected virtual void OnHide () {}
}
