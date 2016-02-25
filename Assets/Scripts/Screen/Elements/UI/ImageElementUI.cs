using UnityEngine;
using System.Collections;

public class ImageElementUI : ScreenElementUI<ImageElement> {

	public override void ApplyElement (ImageElement e) {
		Sprite = e.Sprite;
	}
}
