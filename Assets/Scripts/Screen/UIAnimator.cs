﻿using UnityEngine;
using System;
using System.Collections;

public class UIAnimator : UIElement {

	TemplateAnimation currentAnimation;

	public bool Animating {
		get { return currentAnimation != null && currentAnimation.Animating; }
	}

	public static UIAnimator AttachTo (GameObject obj) {
		UIAnimator anim = obj.GetComponent<UIAnimator> ();
		if (anim != null)
			return anim;
		return obj.AddComponent<UIAnimator> ();
	}

	public bool Animate (TemplateAnimation animation) {
		if (Animating)
			return false;
		animation.Rect = gameObject.GetComponent<RectTransform> ();
		currentAnimation = animation;
		animation.Start ();
		return true;
	}

	/**
	 *	Curves
	 */

	public abstract class Curve {
		public abstract float Get (float x);
	}

	public class Smooth : Curve {
		public override float Get (float x) {
			return (x < 0.5f)
				? 2 * Mathf.Pow(x, 2)
				: -2 * Mathf.Pow(x, 2) + 4 * x - 1;
		}
	}

	public class EaseOutBounce : Curve {
		public override float Get (float x) {
			return x * ( x * (3 * x - 7 ) + 5);
		}
	}

	/**
	 *	Animations
	 */

	public class Expand : TemplateAnimation {

		EaseOutBounce curve = new EaseOutBounce ();

		public Expand (float time) : base (time, (float p) => {
			Rect.SetLocalScale (curve.Get (p));
		}) {}
	}

	public class Peek : TemplateAnimation {
		
		float startPosition = 0;
		Smooth curve = new Smooth ();

		public Peek (float time, float to, Action onEnd=null) : base (time, (float p) => {
			Rect.SetAnchoredPositionY (Mathf.Lerp (startPosition, to, curve.Get (p)));
		}, onEnd) {}

		protected override void OnLoad () {
			startPosition = Rect.anchoredPosition.y;
		}
	}

	public class Slide : TemplateAnimation {

		Smooth curve = new Smooth ();

		public Slide (float time, float to, Action onEnd=null) : base (time, (float p) => { 
			Rect.SetAnchoredPositionX (Mathf.Lerp (0f, to, curve.Get (p)));
		}, onEnd) {}
	}

	public abstract class TemplateAnimation {

		public RectTransform Rect {
			get { return rect; }
			set {
				rect = value;
				OnLoad ();
			}
		}

		public bool Animating { get; private set; }
		protected float time;
		Action<float> anim;
		Action onEnd;
		RectTransform rect;

		public TemplateAnimation (float time, Action<float> anim, Action onEnd=null) {
			this.time = time;
			this.anim = anim;
			this.onEnd = onEnd;
		}

		protected virtual void OnLoad () {}

		public void Start () {
			if (Animating) return;
			Animating = true;
			Co.StartCoroutine (time, anim, () => {
				Animating = false;
				if (onEnd != null)
					onEnd();
			});
		}
	}
}
