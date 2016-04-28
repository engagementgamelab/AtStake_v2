using UnityEngine;
using System.Collections;

/// <summary>
/// Plays audio clips
/// </summary>
public class AudioManager : MonoBehaviour {

	/// <summary>
	/// This is a singleton
	/// </summary>
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

	/// <summary>
	/// Play the audio clip
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	public void Play (AudioClip clip) {
		#if !MUTE_AUDIO
		Source.PlayOneShot (clip);
		#endif
	}
}
