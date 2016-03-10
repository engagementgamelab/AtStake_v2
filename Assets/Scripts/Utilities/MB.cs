﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MB : MonoBehaviour {

	Transform myTransform = null;
	public Transform Transform {
		get { 
			if (myTransform == null) {
				myTransform = transform;
			}
			return myTransform; 
		}
	}

	protected Vector2 V2Position {
		get {
			Vector3 pos = Camera.main.WorldToScreenPoint (Transform.position);
			return new Vector2 (pos.x, Screen.height - pos.y);
		}
	}

	public Vector3 Position {
		get { return Transform.position; }
		set { Transform.position = value; }
	}

	public Vector3 LocalScale {
		get { return Transform.localScale; }
		set { Transform.localScale = value; }
	}

	public Vector3 LocalPosition {
		get { return Transform.localPosition; }
		set { Transform.localPosition = value; }
	}

	public Vector3 LocalEulerAngles {
		get { return Transform.localEulerAngles; }
		set { Transform.localEulerAngles = value; }
	}

	public Transform Parent {
		get { return Transform.parent; }
		set { Transform.SetParent (value); }
	}

	public List<Transform> Children {
		get {
			List<Transform> children = new List<Transform> ();
			foreach (Transform child in Transform) 
				children.Add (child);
			return children;
		}
	}
}
