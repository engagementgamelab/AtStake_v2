using UnityEngine;
using System.Collections;

public class ImageElementUI : ScreenElementUI<ImageElement> {

	public override void ApplyElement (ImageElement e) {
		if (e.SpriteName != "" && Sprite != null && Sprite.name != e.SpriteName)
			Sprite = AssetLoader.LoadIcon (e.SpriteName);
	}
}
