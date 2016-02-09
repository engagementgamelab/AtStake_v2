using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameInstanceUI : UIElement {

	Text nameText;
	Text Name {
		get {
			if (nameText == null) {
				nameText = GetChildComponent<Text> (0);
			}
			return nameText;
		}
	}

	GameInstance gi;

	public void Init (GameInstance gi) {
		this.gi = gi;
	}

	public void AddTextLine (string line) {
		TextLine l = ObjectPool.Instantiate<TextLine> ();
		l.Text.text = line;
		l.Parent = Transform;
	}

	void Update () {
		if (gi.Manager != null) 
			Name.text = gi.Name;
	}

	public void Focus () {
		Name.fontStyle = FontStyle.Bold;
	}

	public void Unfocus () {
		Name.fontStyle = FontStyle.Normal;
	}
}
