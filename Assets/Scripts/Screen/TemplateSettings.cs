﻿using UnityEngine;
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
		get {
			Dictionary<string, Color> combinedColors = new Dictionary<string, Color> ();
			foreach (var color in colors)
				combinedColors[color.Key] = color.Value;
			foreach (var snippetColor in snippetColors) {
				#if UNITY_EDITOR
				if (combinedColors.ContainsKey (snippetColor.Key))
					Debug.LogWarning ("You are using a style snippet to set the color of the screen element with the id '" + snippetColor.Key + "' but also setting the color explicitly. Only the color from the style snippet will be used.");
				#endif
				combinedColors[snippetColor.Key] = snippetColor.Value;
			}
			return combinedColors;
		}
		set { colors = value; }
	}

	Dictionary<string, TextStyle> textStyles = new Dictionary<string, TextStyle> ();
	public Dictionary<string, TextStyle> TextStyles {
		get {
			Dictionary<string, TextStyle> combinedTextStyles = new Dictionary<string, TextStyle> ();
			foreach (var textStyle in textStyles)
				combinedTextStyles[textStyle.Key] = textStyle.Value;
			foreach (var snippetTextStyle in snippetTextStyles) {
				#if UNITY_EDITOR
				if (combinedTextStyles.ContainsKey (snippetTextStyle.Key))
					Debug.LogWarning ("You are using a style snippet to set the color of the screen element with the id '" + snippetTextStyle.Key + "' but also setting the color explicitly. Only the color from the style snippet will be used.");
				#endif
				combinedTextStyles[snippetTextStyle.Key] = snippetTextStyle.Value;
			}
			return combinedTextStyles;
		}
		set { textStyles = value; }
	}

	// Set by StyleSnippets
	Dictionary<string, Color> snippetColors = new Dictionary<string, Color> ();
	public Dictionary<string, Color> SnippetColors {
		get { return snippetColors; }
		set { snippetColors = value; }
	}

	// Set by StyleSnippets
	Dictionary<string, TextStyle> snippetTextStyles = new Dictionary<string, TextStyle> ();
	public Dictionary<string, TextStyle> SnippetTextStyles {
		get { return snippetTextStyles; }
		set { snippetTextStyles = value; }
	}

	// Optionally load in snippets
	public TemplateSettings (params string[] snippets) {
		this.ApplySnippets (snippets);
	}
}