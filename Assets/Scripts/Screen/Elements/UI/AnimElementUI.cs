using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimElementUI : MonoBehaviour {

	public Image image;
	public Text text;

	public void Init (string image, string text="") {
		this.image.sprite = AssetLoader.LoadIcon (image);
		this.text.gameObject.SetActive (text != "");
		this.text.text = text;
	}
}
