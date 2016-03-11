﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Utility class that allows non-MonoBehviour classes to run coroutines

public static class Co {

	public static void StartCoroutine (Func<IEnumerator> func) {
		CoMb.Instance.StartCoroutine (func ());
	}

	public static void WaitForSeconds (float seconds, System.Action onEnd) {
		CoMb.Instance.StartCoroutine (CoWaitForSeconds (seconds, onEnd));
	}

	public static void WaitForFixedUpdate (System.Action onEnd) {
		CoMb.Instance.StartCoroutine (CoWaitForFixedUpdate (onEnd));
	}

	public static void StartCoroutine (float duration, System.Action<float> action, System.Action onEnd=null) {
		CoMb.Instance.StartCoroutine (CoCoroutine (duration, action, onEnd));
	}

	public static void YieldWhileTrue (Func<bool> condition, System.Action onEnd) {
		CoMb.Instance.StartCoroutine (CoYieldWhileTrue (condition, onEnd));
	} 

	public static void InvokeWhileTrue (float time, float rate, Func<bool> condition, System.Action onInvoke, System.Action onEnd=null) {
		
		float duration = time > 0f ? time : rate;

		Co.WaitForSeconds (duration, () => {
			if (condition ()) {
				onInvoke ();
				InvokeWhileTrue (0f, rate, condition, onInvoke, onEnd);
			} else if (onEnd != null) {
				onEnd ();
			}
		});
	}

	public static void Repeat (float rate, int count, System.Action<int> onInvoke, System.Action onEnd=null) {
		Repeat (0f, rate, count, onInvoke, onEnd);
	}

	public static void Repeat (float time, float rate, int count, System.Action<int> onInvoke, System.Action onEnd=null) {
		InvokeWhileTrue (time, rate, () => { return count > 0; }, () => {
			count --;
			onInvoke (count);
			if (count == 0 && onEnd != null)
				onEnd ();
		});
	}

	public static void RepeatAscending (float rate, int max, System.Action<int> onInvoke, System.Action onEnd=null) {
		RepeatAscending (0f, rate, max, onInvoke, onEnd);
	}

	public static void RepeatAscending (float time, float rate, int max, System.Action<int> onInvoke, System.Action onEnd=null) {
		int count = 0;
		InvokeWhileTrue (time, rate, () => { return count < max; }, () => {
			onInvoke (count);
			count ++;
			if (count == max && onEnd != null)
				onEnd ();
		});
	}

	static IEnumerator CoWaitForSeconds (float seconds, System.Action onEnd) {
		float e = 0f;
		while (e < seconds) {
			e += Time.deltaTime;
			yield return null;
		}
		onEnd ();
	}

	static IEnumerator CoWaitForFixedUpdate (System.Action onEnd) {
		yield return new WaitForFixedUpdate ();
		onEnd ();
	}

	static IEnumerator CoCoroutine (float duration, System.Action<float> action, System.Action onEnd) {
		float e = 0f;
		while (e < duration) {
			e += Time.deltaTime;
			action (e / duration);
			yield return null;
		}
		if (onEnd != null)
			onEnd ();
	}

	static IEnumerator CoYieldWhileTrue (Func<bool> condition, System.Action onEnd) {
		while (condition ()) yield return null;
		onEnd ();
	}
}

public class CoMb : MonoBehaviour {

	static CoMb instance = null;
	static public CoMb Instance {
		get {
			if (instance == null) {
				instance = UnityEngine.Object.FindObjectOfType (typeof (CoMb)) as CoMb;
				if (instance == null) {
					GameObject go = new GameObject ("CoMb");
					go.hideFlags = HideFlags.HideInHierarchy;
					DontDestroyOnLoad (go);
					instance = go.AddComponent<CoMb> ();
				}
			}
			return instance;
		}
	}
}