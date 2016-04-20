using UnityEngine;
using System.Collections;

public class AnimationContainer : MB {

	public void Reset () {
		ObjectPool.DestroyChildren<AnimElementUI> (Transform);
	}
}
