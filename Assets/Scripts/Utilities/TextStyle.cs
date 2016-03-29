using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TextTransform {
	Normal, Uppercase, Lowercase
}

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

	public static TextStyle Paragraph {
		get {
			return new TextStyle () {
				FontSize = 18
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
				FontStyle = FontStyle.Italic,
				FontColor = Palette.White,
				TextTransform = TextTransform.Lowercase
			};
		}
	}
}

public static class TextStyleExtension {
	
	public static void ApplyStyle (this Text text, TextStyle style) {
		text.fontStyle = style.FontStyle;
		text.color = style.FontColor;
		text.alignment = style.TextAnchor;
		text.fontSize = style.FontSize;
		if (style.TextTransform == TextTransform.Lowercase) {
			text.text = text.text.ToLower ();
		} else if (style.TextTransform == TextTransform.Uppercase) {
			text.text = text.text.ToUpper ();
		}
	}
}
