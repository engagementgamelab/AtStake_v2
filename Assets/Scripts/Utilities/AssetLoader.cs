using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class AssetLoader {

	static string BackgroundsPath {
		get { return "Sprites/Backgrounds/"; }
	}

	static string IconsPath {
		get { return "Sprite/Icons"; }
	}

	public static Sprite LoadBackground (string name) {
		return LoadSprite (BackgroundsPath + name);
	}

	public static Sprite LoadIcon (string name) {
		return LoadSprite (IconsPath + name);
	}

	static Sprite LoadSprite (string path) {
		Texture2D tex = Resources.Load (path) as Texture2D;
		return Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
	}
}
