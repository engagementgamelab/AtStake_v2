using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TextTransform {
	Normal, Uppercase, Lowercase
}

/// <summary>
/// All the text styles used in the game
/// These get applied to UI Text elements using the extension method ApplyStyle
/// </summary>
public class TextStyle {

	int fontSize = 18;
	public int FontSize {
		get { return fontSize; }
		set { fontSize = value; }
	}

	Color fontColor = Color.black;
	public Color FontColor {
		get { return fontColor; }
		set { fontColor = value; }
	}

	FontStyle fontStyle = FontStyle.Normal;
	public FontStyle FontStyle {
		get { return fontStyle; }
		set { fontStyle = value; }
	}

	TextAnchor textAnchor = TextAnchor.MiddleCenter;
	public TextAnchor TextAnchor {
		get { return textAnchor; }
		set { textAnchor = value; }
	}

	TextTransform textTransform = TextTransform.Normal;
	public TextTransform TextTransform {
		get { return textTransform; }
		set { textTransform = value; }
	}

	public static TextStyle Header {
		get {
			return new TextStyle () {
				FontSize = 28,
				FontStyle = FontStyle.Bold
			};
		}
	}

	public static TextStyle Header2 {
		get { 
			return new TextStyle () {
				FontSize = 26,
				FontColor = Palette.Grey,
				TextTransform = TextTransform.Lowercase
			};
		}
	}

	public static TextStyle Paragraph {
		get { 
			return new TextStyle () {
				FontSize = 18,
				FontColor = Palette.Grey
			};
		}
	}

	public static TextStyle LtParagraph {
		get {
			return new TextStyle () {
				FontSize = 20,
				FontColor = Palette.White
			};
		}
	}

	public static TextStyle Button {
		get {
			return new TextStyle () {
				FontSize = 22,
				FontColor = Palette.Teal,
				TextTransform = TextTransform.Lowercase
			};
		}
	}
	
	public static TextStyle LtButton {
		get {
			return new TextStyle () {
				FontSize = 22,
				FontStyle = FontStyle.BoldAndItalic,
				FontColor = Palette.White,
				TextTransform = TextTransform.Lowercase
			};
		}
	}

	public static TextStyle LargeButton {
		get {
			TextStyle style = LtButton;
			style.FontSize = 30;
			return style;
		}
	}

	public static TextStyle Coin {
		get {
			return new TextStyle () {
				FontSize = 20,
				FontStyle = FontStyle.Bold,
				FontColor = Palette.White
			};
		}
	}
}

public static class TextStyleExtension {
	
	public static void ApplyStyle (this Text text, TextStyle style) {

		// Set font and font style
		text.fontStyle = FontStyle.Normal;
		switch (style.FontStyle) {
			case FontStyle.Normal:
				text.font = AssetLoader.LoadFont ("Normal");
				break;
			case FontStyle.Bold:
				text.font = AssetLoader.LoadFont ("Bold");
				break;
			case FontStyle.Italic:
				text.font = AssetLoader.LoadFont ("Italic");
				break;
			case FontStyle.BoldAndItalic:
				text.font = AssetLoader.LoadFont ("BoldAndItalic");
				break;
		}

		// Set color, alignment, and size
		text.color = style.FontColor;
		text.alignment = style.TextAnchor;
		text.fontSize = style.FontSize;

		// Apply transformations
		if (style.TextTransform == TextTransform.Lowercase) {
			text.text = text.text.ToLower ();
		} else if (style.TextTransform == TextTransform.Uppercase) {
			text.text = text.text.ToUpper ();
		}
	}
}
