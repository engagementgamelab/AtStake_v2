using UnityEngine;
using System.Collections;

public class PlayerInstance : GameInstanceComponent {

	Models.Player data;
	public Models.Player Data {
		get {
			if (data == null) {
				data = new Models.Player ();
			}
			return data;
		}
	}

	[DebuggableMethod]
	public void SetName (string name) {
		Data.Name = name;
	}
}
