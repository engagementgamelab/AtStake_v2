using UnityEngine;
using System.Collections;

public class TextElementUI : ScreenElementUI<TextElement> {

	public override void ApplyElement (TextElement e) {
		Text.text = e.Text;
		ApplyStyle (e.AvatarColor);
	}

	protected override void OnUpdate (TextElement e) {
		Text.text = e.Text;
		if (Style != null)
			ApplyStyle (e.AvatarColor);
	}

	void ApplyStyle (string avatarColor) {
		if (avatarColor != "")
			Style.FontColor = Palette.Avatar.GetColor (avatarColor);
		Text.ApplyStyle (Style);
	}
}
