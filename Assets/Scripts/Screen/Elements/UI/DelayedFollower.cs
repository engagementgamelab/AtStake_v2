using UnityEngine;
using System.Collections;

public class DelayedFollower : MB {

	float speed = 5f;
	Vector3 relPos;
	Transform target;

	void Start(){
		relPos = Transform.localPosition;
		target = Parent;
	}

	void LateUpdate () {
		Vector3 destPos = target.TransformPoint (relPos);
		Transform.position = Vector3.Lerp (Transform.position, destPos, speed * Time.deltaTime);
	}
}
