using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StyleSnippets {

	Dictionary<string, StyleSnippet> snippets;
	Dictionary<string, StyleSnippet> Snippets {
		get {
			if (snippets == null) {
				snippets = new Dictionary<string, StyleSnippet> ();
				snippets.Add ("role_card", RoleCard);
				snippets.Add ("decider_instructions", DeciderInstructions);
				snippets.Add ("logo", Logo);
				snippets.Add ("lt_paragraph", LtParagraph);
				snippets.Add ("next_button", NextButton);
				snippets.Add ("bottom_button", BottomButton);
				snippets.Add ("orange_button", OrangeButton);
				snippets.Add ("green_button", GreenButton);
				snippets.Add ("blue_button", BlueButton);
				snippets.Add ("coins_and_pot", CoinsAndPot);
				snippets.Add ("lt_coins_and_pot", LtCoinsAndPot);
			}
			return snippets;
		}
	}

	public StyleSnippet this[string id] {
		get { return Snippets[id].Clone (); }
	}

	public StyleSnippet Logo {
		get { return new StyleSnippet ("logo", Palette.Transparent.White (0.5f)); }
	}

	public StyleSnippet LtParagraph {
		get { return new StyleSnippet ("text", TextStyle.LtParagraph); }
	}

	public StyleSnippet NextButton {
		get { return new StyleSnippet ("next", Palette.Orange, TextStyle.LtButton); }
	}

	public StyleSnippet BottomButton {
		get { return new StyleSnippet ("button", Palette.LtBlue, TextStyle.LargeButton); }
	}

	public StyleSnippet OrangeButton {
		get { return new StyleSnippet ("button", Palette.Orange, TextStyle.LtButton); }
	}

	public StyleSnippet GreenButton {
		get { return new StyleSnippet ("button", Palette.Green, TextStyle.LtButton); }
	}

	public StyleSnippet BlueButton {
		get { return new StyleSnippet ("button", Palette.Blue, TextStyle.LtButton); }
	}

	public StyleSnippet RoleCard {
		get {

			TextStyle title = new TextStyle () {
				FontSize = 24,
				FontColor = Palette.Grey
			};

			TextStyle agendaTitle = new TextStyle () {
				FontSize = 20,
				FontColor = Palette.LtTeal
			};

			TextStyle agendaItem = new TextStyle () {
				FontSize = 16
			};
			
			return new StyleSnippet () {
				Snippets = new List<StyleSnippet> () {
					new StyleSnippet ("rc_title", title),
					new StyleSnippet ("rc_bio", TextStyle.Paragraph),
					new StyleSnippet ("rc_agenda_title", agendaTitle),
					new StyleSnippet ("rc_item0", agendaItem),
					new StyleSnippet ("rc_item1", agendaItem),
					new StyleSnippet ("rc_reward0", TextStyle.Coin),
					new StyleSnippet ("rc_reward1", TextStyle.Coin)
				}
			};
		}
	}

	public StyleSnippet DeciderInstructions {
		get {

			TextStyle readAloud = new TextStyle () {
				FontSize = 26,
				FontColor = Palette.Grey
			};

			return new StyleSnippet () {
				Snippets = new List<StyleSnippet> () {
					new StyleSnippet ("instructions_read", readAloud),
					new StyleSnippet ("decider_instructions_out_loud", TextStyle.LtParagraph),
					new StyleSnippet ("decider_instructions", TextStyle.Paragraph)
				}
			};
		}
	}

	public StyleSnippet CoinsAndPot {
		get {

			TextStyle style = new TextStyle () {
				FontSize = 24,
				FontColor = Palette.White
			};

			return new StyleSnippet () {
				Snippets = new List<StyleSnippet> () {
					new StyleSnippet ("coins", style),
					new StyleSnippet ("pot", style)
				}
			};
		}
	}

	public StyleSnippet LtCoinsAndPot {
		get {

			TextStyle style = new TextStyle () {
				FontSize = 24,
				FontColor = Palette.Grey
			};

			return new StyleSnippet () {
				Snippets = new List<StyleSnippet> () {
					new StyleSnippet ("coins", style),
					new StyleSnippet ("pot", style)
				}
			};
		}
	}
}

public class StyleSnippet {

	string id;
	public string Id {
		get { return id; }
		set { id = value; }
	}

	bool hasColor = false;
	Color color;
	public Color Color {
		get { return color; }
		set { 
			color = value; 
			hasColor = true;
		}
	}

	TextStyle textStyle;
	public TextStyle TextStyle {
		get { return textStyle; }
		set { textStyle = value; }
	}

	List<StyleSnippet> snippets;
	public List<StyleSnippet> Snippets {
		get { return snippets; }
		set { snippets = value; }
	}

	public StyleSnippet (string id, Color color, TextStyle textStyle=null) {
		Id = id;
		Color = color;
		TextStyle = textStyle;
	}

	public StyleSnippet (string id, TextStyle textStyle) {
		Id = id;
		TextStyle = textStyle;
	}

	public StyleSnippet () {}

	public void Apply (TemplateSettings settings) {

		if (Snippets != null) {
			foreach (StyleSnippet snippet in Snippets)
				snippet.Apply (settings);
		}

		if (hasColor) {
			settings.SnippetColors.Add (Id, Color);
		}
		if (TextStyle != null) {
			settings.SnippetTextStyles.Add (Id, TextStyle);
		}
	}

	public StyleSnippet Clone () {
		StyleSnippet s = new StyleSnippet () {
			Id = Id,
			TextStyle = TextStyle,
			Snippets = Snippets
		};
		if (hasColor)
			s.Color = Color;
		return s;
	}
}

public static class StyleSnippetExtensions {

	public static void ApplySnippets (this TemplateSettings settings, StyleSnippet[] snippets) {

		foreach (StyleSnippet snippet in snippets)
			snippet.Apply (settings);
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
				StyleSnippet outSnippet = snippets[snippetId];
				outSnippets.Add (outSnippet);
			} else {
				for (int j = 0; j < elementIds.Length; j ++) {
					StyleSnippet outSnippet = snippets[snippetId];
					outSnippet.Id = elementIds[j];
					outSnippets.Add (outSnippet);
				}
			}
		}

		settings.ApplySnippets (outSnippets.ToArray ());
	}
}
