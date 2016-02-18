using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
}
