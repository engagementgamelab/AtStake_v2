using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameScreen {

	public abstract Dictionary<string, ScreenElement> Elements { get; }
	Dictionary<string, ScreenElement> dynamicElements = new Dictionary<string, ScreenElement> ();

	Transform canvas;
	protected GameInstance game;

	public void Init (GameInstance game, Transform canvas) {
		this.game = game;
		this.canvas = canvas;
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

	protected virtual void OnShow () {}
	protected virtual void OnHide () {}

	protected T GetScreenElement<T> (string id) where T : ScreenElement {
		return Elements[id] as T;
	}

	protected void AddElement (string id, ScreenElement element) {
		dynamicElements.Add (id, element);
		RenderDynamic ();
	}

	protected void RemoveElement (string id) {
		dynamicElements.Remove (id);
		RenderDynamic ();
	}

	// Routing
	public void GotoScreen (string id) {
		game.Screens.SetScreen (id);
	}

	// not being used rn - could it be removed?
	public void GoBack () {
		game.Screens.SetScreenPrevious ();
	}
}
