using UnityEngine;
using System.Collections;

public class RadioListElementUI : ListElementUI<ButtonElementUI, ButtonElement> {

	SpacerElementUI spacer;
	SpacerElementUI Spacer {
		get {
			if (spacer == null) {
				spacer = ObjectPool.Instantiate<SpacerElementUI> ();
				spacer.Parent = Transform;
				spacer.Transform.Reset ();
				spacer.Height = 12;
			}
			return spacer;
		}
	}

	protected override void OnUpdateListElements () {
		Spacer.Transform.SetSiblingIndex (ChildElements.Count);
		ButtonElementUI confirm = GetChildElement ("confirm");
		confirm.Transform.SetSiblingIndex (ChildElements.Count+1);
	}

	public override void RemoveElement (ListElement<ButtonElement> e) {
		ObjectPool.Destroy<SpacerElementUI> (spacer);
		base.RemoveElement (e);
	}
}
