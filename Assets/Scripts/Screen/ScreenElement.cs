using UnityEngine;
using System.Collections;
using Views;

public abstract class ScreenElement : GameInstanceComponent {
	
	public delegate void OnUpdate (ScreenElement e);

	public OnUpdate onUpdate;

	public void Init (GameInstanceBehaviour behaviour) {
		base.Init (behaviour);
	}

	protected void SendUpdateMessage () {
		if (onUpdate != null)
			onUpdate (this);
	}
}
