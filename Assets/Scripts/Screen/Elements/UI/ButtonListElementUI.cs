using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonListElementUI : ListElementUI<ButtonElementUI, ButtonElement> {

	TextStyle style = TextStyle.LtButton;
	public override TextStyle Style {
		get { return style; }
		set {
			style = value;
			foreach (ButtonElementUI child in ChildElements)
				child.Style = style;
		}
	}
}
