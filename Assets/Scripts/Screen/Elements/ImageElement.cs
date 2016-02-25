using UnityEngine;

public class ImageElement : ScreenElement {

	public readonly Sprite Sprite;

	public ImageElement (string spriteName) {
		Sprite = AssetLoader.LoadIcon (spriteName);
	}
}
