using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TemplateSettings {

	// -- Top & bottom bars

	public const float TallBar = 92;
	public const float ShortBar = 24;

	public float TopBarHeight { get; set; }
	public float BottomBarHeight { get; set; }

	Color topBarColor = Palette.White;
	public Color TopBarColor {
		get { return topBarColor; }
		set { topBarColor = value; }
	}

	Color bottomBarColor = Palette.White;
	public Color BottomBarColor {
		get { return bottomBarColor; }
		set { bottomBarColor = value; }
	}

	// -- Background

	Color backgroundColor = Palette.White;
	public Color BackgroundColor {
		get { return backgroundColor; }
		set { backgroundColor = value; }
	}

	public string BackgroundImage { get; set; }

	// -- Coins

	public bool PotEnabled { get; set; }
	public bool CoinsEnabled { get; set; }

	// -- Styling

	Dictionary<string, Color> colors = new Dictionary<string, Color> ();
	public Dictionary<string, Color> Colors {
		get { return colors; }
		set { colors = value; }
	}

	Dictionary<string, TextStyle> textStyles = new Dictionary<string, TextStyle> ();
	public Dictionary<string, TextStyle> TextStyles {
		get { return textStyles; }
		set { textStyles = value; }
	}
}