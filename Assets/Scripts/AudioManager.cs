using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	static AudioManager instance = null;
		static public AudioManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (AudioManager)) as AudioManager;
				if (instance == null) {
					GameObject go = new GameObject ("AudioManager");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<AudioManager>();
				}
			}
			return instance;
		}
	}

	AudioSource source = null;
	AudioSource Source {
		get {
			if (source == null) {
				source = GetComponent<AudioSource> ();
			}
			return source;
		}
	}

	public void Play (AudioClip clip) {
		Source.PlayOneShot (clip);
	}
}
