using UnityEngine;
using System.Collections;

public class RadioListElementUI : ListElementUI<ButtonElementUI, ButtonElement> {

	protected override void OnUpdateListElements () {
		ButtonElementUI confirm = GetChildElement ("confirm");
		confirm.Transform.SetSiblingIndex (ChildElements.Count-1);
	}
}
