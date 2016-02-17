using UnityEngine;

public static class Palette {

	public static Color White {
		get { return Color.white; }
	}

	static Color Color255 (float r, float g, float b) {
		return new Color (r/255f, g/255f, b/255f);
	}
}
