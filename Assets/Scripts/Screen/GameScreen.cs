using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameScreen {

	public abstract List<ScreenElement> Elements { get; }

	Transform canvas;

	public void Init (Transform canvas) {
		this.canvas = canvas;
	}
	
	public void Show () {
		foreach (ScreenElement element in Elements) {
			element.Render ().SetParent (canvas);
		}
	}

	public void Hide () {
		foreach (ScreenElement element in Elements) {
			element.Remove ();
		}
	}
}
