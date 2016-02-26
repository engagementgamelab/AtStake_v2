using UnityEngine;
using System.Collections;
using Views;

public abstract class ScreenElement : GameInstanceComponent {
	
	bool active = true;
	public bool Active {
		get { return active; }
		set { 
			active = value;
			SendUpdateMessage ();
		}
	}

	public delegate void OnUpdate (ScreenElement e);

	public OnUpdate onUpdate;

	protected void SendUpdateMessage () {
		if (onUpdate != null)
			onUpdate (this);
	}
}
