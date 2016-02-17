using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenTemplate : MonoBehaviour {

	public Image backgroundColor;
	public Image backgroundImage;

	Color BackgroundColor {
		set { backgroundColor.color = value; }
	}

	Sprite BackgroundImage {
		set { backgroundImage.sprite = value; }
	}

	public void SetBackground (Color bgColor, string bgImage) {
		BackgroundColor = bgColor;
		BackgroundImage = AssetLoader.LoadBackground (bgImage);
	}
}
