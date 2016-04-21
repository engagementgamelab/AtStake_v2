using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimElementUI : MB {

	public Image image;
	public Text text;

	UIAnimator anim = null;
	UIAnimator Anim {
		get {
			if (anim == null) {
				anim = GetComponent<UIAnimator> ();
				anim.AllowSimultaneous = true;
			}
			return anim;
		}
	}

	string spriteName;
	public string SpriteName {
		get { return spriteName; }
		set {
			spriteName = value;
			image.sprite = AssetLoader.LoadIcon (value);
		}
	}

	string avatarName;
	public string AvatarName {
		get { return avatarName; }
		set {
			avatarName = value;
			image.sprite = AssetLoader.LoadAvatar (value);
		}
	}

	public string Text {
		get { return text.text; }
		set { text.text = value; }
	}

	public Vector2 Size {
		get { return RectTransform.sizeDelta; }
		set { RectTransform.sizeDelta = value; }
	}

	public Vector2 TextPadding {
		get { return text.GetComponent<RectTransform> ().sizeDelta; }
		set { text.GetComponent<RectTransform> ().sizeDelta = value; }
	}

	RectTransform rectTransform = null;
	public RectTransform RectTransform {
		get {
			if (rectTransform == null) {
				rectTransform = GetComponent<RectTransform> ();
			}
			return rectTransform;
		}
	}

	public static AnimElementUI Create (Transform parent, Vector3 position) {
		AnimElementUI anim = ObjectPool.Instantiate<AnimElementUI> ();
		anim.Transform.SetParent (parent);
		anim.Transform.Reset ();
		return anim;
	}

	public void Destroy () {
		ObjectPool.Destroy<AnimElementUI> (Transform);
	}

	public void Animate (UIAnimator.UIAnimation animation, RectTransform rect=null) {
		Anim.Animate (animation, rect);
	}

	void OnDisable () {
		image.sprite = null;
		Text = "";
		Size = new Vector2 (100, 100);
		TextPadding = Vector2.zero;
	}
}
