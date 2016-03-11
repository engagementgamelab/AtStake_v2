using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Utility class for running coroutines. Allows coroutines to be run in classes not derived from MonoBehaviour. Uses anonymous functions.
/// </summary>
public static class Co {

	/// <summary>
	/// Runs a coroutine
	/// </summary>
	/// <param name="func">The coroutine to run</param>
	public static void StartCoroutine (Func<IEnumerator> func) {
		CoMb.Instance.StartCoroutine (func ());
	}

	/// <summary>
	/// Waits for an amount of time, and then runs a function
	/// </summary>
	/// <param name="seconds">The amount of time to wait</param>
	/// <param name="onEnd">The function to run</param>
	public static void WaitForSeconds (float seconds, System.Action onEnd) {
		CoMb.Instance.StartCoroutine (CoWaitForSeconds (seconds, onEnd));
	}

	/// <summary>
	/// Waits one frame, and then runs a function
	/// </summary>
	/// <param name="onEnd">The function to run</param>
	public static void WaitForFixedUpdate (System.Action onEnd) {
		CoMb.Instance.StartCoroutine (CoWaitForFixedUpdate (onEnd));
	}

	/// <summary>
	/// Runs a function every frame for an amount of time
	/// </summary>
	/// <param name="duration">The amount of time to run (in seconds)</param>
	/// <param name="action">The function to call every frame. Passes the % of time that has elapsed as a float</param>
	/// <param name="onEnd">(option) A function to run after the time has elapsed</param>
	public static void StartCoroutine (float duration, System.Action<float> action, System.Action onEnd=null) {
		CoMb.Instance.StartCoroutine (CoCoroutine (duration, action, onEnd));
	}

	/// <summary>
	/// Waits until an expression evaluates as false, and then calls a function
	/// </summary>
	/// <param name="condition">The expression to evaluate. Continues waiting as long as the expression returns true.</param>
	/// <param name="onEnd">The function to call when the expression returns false</param>
	public static void YieldWhileTrue (Func<bool> condition, System.Action onEnd) {
		CoMb.Instance.StartCoroutine (CoYieldWhileTrue (condition, onEnd));
	} 

	/// <summary>
	/// Repeatedly invokes a function as long as the condition is met
	/// </summary>
	/// <param name="time">The initial delay before invoking begins</param>
	/// <param name="rate">The delay between invoke calls</param>
	/// <param name="condition">The expression to evaluate. When 'condition' is false, the coroutine stops.</param>
	/// <param name="onEnd">(optional) A function to run after the coroutine has finished</param>
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

	/// <summary>
	/// Repeats an action a given number of times (like a 'for' loop with a delay between each iteration). Counts down to zero.
	/// </summary>
	/// <seeAlso cref="RepeatAscending" />
	/// <param name="time">(optional) The initial delay before counting begins</param>
	/// <param name="rate">The delay between actions</param>
	/// <param name="count">The number of times to repeat</param>
	/// <param name="onInvoke">The function to call. Passes in the loop index.</param>
	/// <param name="onEnd">(optional) The function to call when the count is 0</param>
	public static void Repeat (float time, float rate, int count, System.Action<int> onInvoke, System.Action onEnd=null) {
		InvokeWhileTrue (time, rate, () => { return count > 0; }, () => {
			count --;
			onInvoke (count);
			if (count == 0 && onEnd != null)
				onEnd ();
		});
	}

	public static void Repeat (float rate, int count, System.Action<int> onInvoke, System.Action onEnd=null) {
		Repeat (0f, rate, count, onInvoke, onEnd);
	}

	/// <summary>
	/// Repeats an action a given number of times (like a 'for' loop with a delay between each iteration). Counts up from zero.
	/// </summary>
	/// <seeAlso cref="Repeat" />
	/// <param name="time">(optional) The initial delay before counting begins</param>
	/// <param name="rate">The delay between actions</param>
	/// <param name="count">The number of times to repeat</param>
	/// <param name="onInvoke">The function to call. Passes in the loop index.</param>
	/// <param name="onEnd">(optional) The function to call when the count is 0</param>
	public static void RepeatAscending (float time, float rate, int max, System.Action<int> onInvoke, System.Action onEnd=null) {
		int count = 0;
		InvokeWhileTrue (time, rate, () => { return count < max; }, () => {
			onInvoke (count);
			count ++;
			if (count == max && onEnd != null)
				onEnd ();
		});
	}

	public static void RepeatAscending (float rate, int max, System.Action<int> onInvoke, System.Action onEnd=null) {
		RepeatAscending (0f, rate, max, onInvoke, onEnd);
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