using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInstanceManager : MonoBehaviour {

	static GameInstanceManager instance = null;
		static public GameInstanceManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (GameInstanceManager)) as GameInstanceManager;
				if (instance == null) {
					GameObject go = new GameObject ("GameInstanceManager");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<GameInstanceManager>();
				}
			}
			return instance;
		}
	}

	List<GameInstance> instances = new List<GameInstance> ();
	int focused = -1;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Q)) {
			GameInstance i = ObjectPool.Instantiate<GameInstance> ();
			i.transform.SetParent (transform);
			instances.Add (i);
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			FocusInstance (0);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			FocusInstance (1);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			FocusInstance (2);
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			FocusInstance (3);
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			FocusInstance (4);
		}
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			FocusInstance (5);
		}
	}

	void FocusInstance (int index) {
		if (focused != -1) {
			instances[focused].Unfocus ();
		}
		if (index >= 0 && index < instances.Count) {
			instances[index].Focus ();
			focused = index;
		}
	}
}
