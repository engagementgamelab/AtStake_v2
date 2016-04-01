using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class AssetLoader {

	static string BackgroundsPath {
		get { return "Sprites/Backgrounds/"; }
	}

	static string IconsPath {
		get { return "Sprites/Icons/"; }
	}

	static string FontsPath {
		get { return "Fonts/"; }
	}

	public static string GetAvatarFilename (string color) {
		return "avatar_" + color;
	}

	public static Sprite LoadBackground (string name) {
		return LoadSprite (BackgroundsPath + name);
	}

	public static Sprite LoadIcon (string name) {
		return LoadSprite (IconsPath + name);
	}

	public static Sprite LoadAvatar (string color) {
		return LoadIcon (GetAvatarFilename (color));
	}

	static Sprite LoadSprite (string path) {
		Texture2D tex = Resources.Load (path) as Texture2D;
		try {
			return Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
		} catch (System.NullReferenceException e) {
			throw new System.Exception ("Could not load the sprite at the path " + path + "\n" + e);
		}
	}

	public static Font LoadFont (string name) {
		try {
			return (Font)Resources.Load (FontsPath + name);
		} catch {
			throw new System.Exception ("Could not find a font with the name '" + name + "'");
		}
	}
}
