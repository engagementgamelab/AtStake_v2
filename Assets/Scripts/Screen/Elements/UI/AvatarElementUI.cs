using UnityEngine;
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

	public override void ApplyElement (AvatarElement e) {
		
		avatar.sprite = AssetLoader.LoadIcon ("avatar_" + e.Color);

		playerName.text = e.Name;
		playerRole.gameObject.SetActive (e.Role != "");
		playerRole.text = e.Role;

		roleStyle.FontColor = Palette.Avatar.GetColor (e.Color);
		playerName.ApplyStyle (nameStyle);
		playerRole.ApplyStyle (roleStyle);
	}
}
