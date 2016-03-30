using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AvatarElementUI : ScreenElementUI<AvatarElement> {

	public Image image;
	public Text text;

	public override void ApplyElement (AvatarElement e) {
		image.sprite = AssetLoader.LoadIcon ("avatar_" + e.Color);
		text.text = e.Name;
	}
}
