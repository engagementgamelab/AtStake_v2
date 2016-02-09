using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameScreen {

	public abstract Dictionary<string, ScreenElement> Elements { get; }

	Transform canvas;
	protected GameInstance game;

	public void Init (GameInstance game, Transform canvas) {
		this.game = game;
		this.canvas = canvas;
	}
	
	public void Show () {
		foreach (var element in Elements) {
			element.Value.Render (this).SetParent (canvas);
		}
		OnShow ();
	}

	public void Hide () {
		foreach (var element in Elements) {
			element.Value.Remove ();
		}
		OnHide ();
	}

	protected virtual void OnShow () {}
	protected virtual void OnHide () {}

	public void GotoScreen (string id) {
		game.Screens.SetScreen (id);
	}

	// not being used rn - could it be removed?
	public void GoBack () {
		game.Screens.SetScreenPrevious ();
	}

	protected T GetScreenElement<T> (string id) where T : ScreenElement {
		return Elements[id] as T;
	}
}
