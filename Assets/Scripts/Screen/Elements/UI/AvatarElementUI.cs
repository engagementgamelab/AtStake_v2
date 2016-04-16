﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AvatarElementUI : ScreenElementUI<AvatarElement> {

	public Image avatar;
	public Text playerName;
	public Text playerRole;

	TextStyle nameStyle = new TextStyle () {
		TextTransform = TextTransform.Lowercase,
		FontColor = Palette.Grey,
		FontSize = 18
	};

	TextStyle roleStyle = new TextStyle () {
		TextTransform = TextTransform.Lowercase,
		FontSize = 18,
		FontStyle = FontStyle.Bold
	};

	void OnEnable () { Animate (new UIAnimator.Expand (1.5f)); }

	public override void ApplyElement (AvatarElement e) {

		Transform.localScale = Vector3.zero;

		// Set the sprite based on the avatar color
		avatar.sprite = AssetLoader.LoadAvatar (e.Color);

		// Set the content
		playerName.text = e.Name;
		playerRole.gameObject.SetActive (e.Role != "");
		playerRole.text = e.Role;

		// Style the text
		roleStyle.FontColor = Palette.Avatar.GetColor (e.Color);
		playerName.ApplyStyle (nameStyle);
		playerRole.ApplyStyle (roleStyle);
	}
}
