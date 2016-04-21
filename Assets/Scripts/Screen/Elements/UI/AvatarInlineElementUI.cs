using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AvatarInlineElementUI : ScreenElementUI<AvatarElement> {

	public Image avatar;
	public Text playerName;
	public Text coinCount;
	bool spinning = false;

	TextStyle nameStyle = new TextStyle () {
		TextTransform = TextTransform.Lowercase,
		FontColor = Palette.Grey,
		FontSize = 32
	};

	TextStyle coinStyle = new TextStyle () {
		TextTransform = TextTransform.Lowercase,
		FontSize = 32,
		FontStyle = FontStyle.BoldAndItalic,
		FontColor = Palette.LtTeal
	};

	public override void ApplyElement (AvatarElement e) {

		// Set the sprite based on the avatar color
		avatar.sprite = AssetLoader.LoadAvatar (e.Color);

		// Set the content
		playerName.text = e.Name;

		// Style the text
		playerName.ApplyStyle (nameStyle);
		coinCount.ApplyStyle (coinStyle);

		coinCount.text = e.CoinCount.ToString ();
		Spin ();
	}

	void Spin () {
		if (!spinning) {
			spinning = true;
			Co.WaitForSeconds (Random.Range (5, 8), () => {
				RectTransform r = avatar.GetComponent<RectTransform> ();
				if (Random.value > 0.5f) {
					Animate (new UIAnimator.Spin (1f), r);
				} else {
					Animate (new UIAnimator.Bump (1f, 1.25f), r);
				}
				spinning = false;
				Spin ();
			});
		}
	}
}
