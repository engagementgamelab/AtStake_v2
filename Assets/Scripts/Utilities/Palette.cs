using UnityEngine;

public static class Palette {

	static Color Color255 (float r, float g, float b) {
		return new Color (r/255f, g/255f, b/255f);
	}

	public static Color White {
		get { return Color.white; }
	}

	public static Color Black {
		get { return Color.black; }
	}

	public static Color Grey {
		get { return new Color (0.5f, 0.5f, 0.5f); }
	}

	public static Color Orange {
		get { return Color255 (255, 186, 0); }
	}

	public static Color Blue {
		get { return Color255 (32, 181, 230); }
	}

	public static Color LtBlue {
		get { return Color255 (0, 205, 217); }
	}

	public static Color Teal {
		get { return Color255 (28, 187, 180); }
	}

	public static Color LtTeal {
		get { return Color255 (0, 213, 180); }
	}

	public static Color Green {
		get { return Color255 (211, 231, 73); }
	}

	public static Color Pink {
		get { return Color255 (255, 44, 118); }
	}

	public static class Avatar {

		public static Color GetColor (string id) {
			
			id = id.ToLower ();

			switch (id) {
				case "yellow": return Yellow;
				case "orange": return Orange;
				case "green": return Green;
				case "pink": return Pink;
				case "red": return Red;
				default: 
					Debug.LogWarning ("Could not find an avatar color called '" + id + "'");
					return Palette.Grey;
			}
		}

		public static Color Yellow {
			get { return Color255 (255, 175, 0); }
		}

		public static Color Orange {
			get { return Color255 (255, 138, 0); }
		}

		public static Color Green {
			get { return Color255 (0, 221, 197); }
		}

		public static Color Red {
			get { return Color255 (255, 71, 0); }
		}

		public static Color Pink {
			get { return Color255 (255, 28, 87); }
		}
	}

	public static class Transparent {

		public static Color White (float alpha) {
			Color c = Palette.White;
			c.a = alpha;
			return c;
		}

		public static Color Black (float alpha) {
			Color c = Palette.Black;
			c.a = alpha;
			return c;
		}
	}
}
