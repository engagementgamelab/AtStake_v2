using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AvatarElementUI : ScreenElementUI<AvatarElement> {

	public Image avatar;
	public Text name;

	public override void ApplyElement (AvatarElement e) {
		avatar.sprite = AssetLoader.LoadIcon ("avatar_" + e.Color);
		name.text = e.Name;
	}
}
