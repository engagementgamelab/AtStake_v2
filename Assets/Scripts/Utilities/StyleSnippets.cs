using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StyleSnippets {

	Dictionary<string, StyleSnippet> snippets;
	public Dictionary<string, StyleSnippet> Snippets {
		get {
			if (snippets == null) {
				snippets = new Dictionary<string, StyleSnippet> ();
				snippets.Add ("role_card", RoleCard);
				snippets.Add ("logo", Logo);
				snippets.Add ("lt_paragraph", LtParagraph);
			}
			return snippets;
		}
	}

	public StyleSnippet RoleCard {
		get {
			return new StyleSnippet () {
				Colors = new Dictionary<string, Color> () {

				},
				TextStyles = new Dictionary<string, TextStyle> () {

				}
			};
		}
	}

	public StyleSnippet Logo {
		get {
			return new StyleSnippet () {
				Colors = new Dictionary<string, Color> () {
					{ "logo", Palette.Transparent.White (0.5f) }
				}
			};
		}
	}

	public StyleSnippet LargeButton {
		get {
			return new StyleSnippet () {
				Colors = new Dictionary<string, Color> () {

				}
			};
		}
	}

	public StyleSnippet LtParagraph {
		get {
			return new StyleSnippet () {
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "text", TextStyle.LtParagraph }
				}
			};
		}
	}
}

public class StyleSnippet {

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

	// Will only work for snippets that have a single id (where the dictionaries basically operate like key value)
	public void ApplyId (string id) {
		Color c = Color.white;
		TextStyle t = null;
		foreach (var color in Colors)
			c = color.Value;
		foreach (var textStyle in TextStyles)
			t = textStyle.Value;
		Colors = new Dictionary<string, Color> () {{ id, c }};
		TextStyles = new Dictionary<string, TextStyle> () {{ id, t }};
	}
}

public static class StyleSnippetExtensions {

	public static void ApplySnippet (this TemplateSettings settings, StyleSnippet snippet) {

		foreach (var color in snippet.Colors)
			settings.SnippetColors.Add (color.Key, color.Value);

		foreach (var textStyle in snippet.TextStyles)
			settings.SnippetTextStyles.Add (textStyle.Key, textStyle.Value);
	}

	public static void ApplySnippets (this TemplateSettings settings, StyleSnippet[] snippets) {
		foreach (StyleSnippet snippet in snippets)
			settings.ApplySnippet (snippet);
	}

	// Applies snippets to the given settings 
	public static void ApplySnippets (this TemplateSettings settings, params string[] ids) {
		
		StyleSnippets snippets = new StyleSnippets ();
		List<StyleSnippet> outSnippets = new List<StyleSnippet> ();
		
		for (int i = 0; i < ids.Length; i ++) {

			string[] elementSnippet = ids[i].Split ('|');
			string snippetId;
			string[] elementIds = null;

			if (elementSnippet.Length > 1) {

				snippetId = elementSnippet[0];
				elementIds = new string[elementSnippet.Length-1];
				for (int j = 1; j < elementSnippet.Length; j ++)
					elementIds[j-1] = elementSnippet[j];

			} else {
				snippetId = ids[i];
			}


			if (elementIds == null) {
				StyleSnippet outSnippet = snippets.Snippets[snippetId];
				outSnippets.Add (outSnippet);
			} else {
				for (int j = 0; j < elementIds.Length; j ++) {
					StyleSnippet outSnippet = snippets.Snippets[snippetId];
					outSnippet.ApplyId (elementIds[j]);
					outSnippets.Add (outSnippet);
				}
			}
		}

		settings.ApplySnippets (outSnippets.ToArray ());
	}
}
