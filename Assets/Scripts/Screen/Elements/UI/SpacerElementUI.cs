using UnityEngine;
using System.Collections;

public class SpacerElementUI : UIElement {

	public float Height {
		get { return Layout.preferredHeight; }
		set { Layout.preferredHeight = value; }
	}
}
