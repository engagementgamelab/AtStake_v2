using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Templates {

	public class Games : Template {

		protected override TemplateSettings LoadSettings () {
			return new TemplateSettings ("logo", "lt_paragraph|searching|client_instructions", "orange_button|confirm") {
				BackgroundColor = Palette.Teal,
				TextStyles = new Dictionary<string, TextStyle> () {
					{ "game_list", TextStyle.Button }
				}
			};
		}

		TextElementUI searching;
		TextElementUI Searching {
			get {
				if (searching == null)
					searching = GetElement<TextElementUI> ("searching");
				return searching;
			}
		}

		string searchingText;
		int ellipsisIndex = 0;

		protected override void OnLoadView () {
			searchingText = Searching.Text.text;
		}

		protected override void OnInputEnabled () {
			Co.InvokeWhileTrue (0.3f, () => { return Loaded; }, () => {
				if (Searching.Text.text.Contains ("Searching")) {
					string text = searchingText;
					for (int i = 0; i < ellipsisIndex; i ++)
						text += ".";
					if (ellipsisIndex < 3)
						ellipsisIndex ++;
					else
						ellipsisIndex = 0;
					Searching.Text.text = text;
				}
			});
		}
	}
}