using UnityEngine;

public class ImageElement : ScreenElement {

	public readonly Sprite Sprite;

	public ImageElement (string spriteName) {
		Sprite = spriteName == "" ? null : AssetLoader.LoadIcon (spriteName);
	}
}
