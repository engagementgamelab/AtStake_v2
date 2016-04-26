using UnityEngine;
using System.Collections;
using Views;

public abstract class ScreenElement : GameInstanceComponent {
	
	bool active = true;
	public bool Active {
		get { return active; }
		set { 
			if (active != value) {
				active = value;
				SendUpdateMessage ();
			}
		}
	}

	protected string audioClip;
	public delegate void OnUpdate (ScreenElement e);

	public OnUpdate onUpdate;

	protected void SendUpdateMessage () {
		if (onUpdate != null)
			onUpdate (this);
	}

	public virtual void PlayAudio () { 
		if (!string.IsNullOrEmpty (audioClip))
			Game.Audio.Play (audioClip); 
	}
}
