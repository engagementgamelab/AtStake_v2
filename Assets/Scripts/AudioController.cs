using UnityEngine;
using System.Collections;

public class AudioController : GameInstanceBehaviour {

	AudioManager manager;

	public void Init () { manager = AudioManager.Instance; }

	public void Play (string clipName) {
		manager.Play (AssetLoader.LoadAudio (clipName));
	}
}
