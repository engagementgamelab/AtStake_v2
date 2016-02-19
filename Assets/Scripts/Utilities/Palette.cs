using UnityEngine;

public static class Palette {

	static Color Color255 (float r, float g, float b) {
		return new Color (r/255f, g/255f, b/255f);
	}

	public static Color White {
		get { return Color.white; }
	}

	public static Color Orange {
		get { return Color255 (255, 119, 0); }
	}

	public static Color Blue {
		get { return Color255 (0, 0, 255); }
	}

	public static class Transparent {

		public static Color White {
			get { return new Color (1, 1, 1, 0.1f); }
		}

		public static Color Black {
			get { return new Color (0, 0, 0, 0.1f); }
		}
	}
}
