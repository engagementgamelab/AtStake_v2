using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenTemplate : MonoBehaviour {

	public Image backgroundColor;
	public Image backgroundImage;
	public Image topBar;

	public Button backButton;

	public RectTransform pot;
	public RectTransform coins;

	public RectTransform contentArea;

	Color BackgroundColor {
		set { backgroundColor.color = value; }
	}

	Sprite BackgroundImage {
		set { backgroundImage.sprite = value; }
	}

	Color TopBarColor {
		set { topBar.color = value; }
	}

	int PotValue {
		set { pot.GetChild (0).GetComponent<Text> ().text = value.ToString (); }
	}

	int CoinsValue {
		set { coins.GetChild (0).GetComponent<Text> ().text = value.ToString (); }
	}

	public void SetBackground (Color bgColor, string bgImage) {
		BackgroundColor = bgColor;
		BackgroundImage = AssetLoader.LoadBackground (bgImage);
	}

	public void SetTopBar (bool enabled, Color topBarColor=new Color()) {
		topBar.gameObject.SetActive (enabled);
		TopBarColor = topBarColor;
	}

	public void SetBackButtonEnabled (bool enabled) {
		backButton.gameObject.SetActive (enabled);
	}

	public void SetScoresEnabled (bool enabled) {
		SetScoresEnabled (enabled, enabled);
	}

	public void SetScoresEnabled (bool potEnabled, bool coinsEnabled) {
		pot.gameObject.SetActive (potEnabled);
		coins.gameObject.SetActive (coinsEnabled);
	}

	public void SetScores (int potValue, int coinsValue) {
		PotValue = potValue;
		CoinsValue = coinsValue;
	}

	void Awake () {
		SetBackground (Palette.White, "applause-bg");
		// SetTopBar (false, Palette.Orange);
		// SetBackButtonEnabled (false);
		SetScoresEnabled (false);
	}
}
