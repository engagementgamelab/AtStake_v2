﻿using UnityEngine;
using System;
using System.Collections;

public class UIAnimator : UIElement {

	UIAnimation currentAnimation;

	public bool Animating {
		get { return currentAnimation != null && currentAnimation.Animating; }
	}

	bool allowSimultaneous = false;
	public bool AllowSimultaneous {
		get { return allowSimultaneous; }
		set { allowSimultaneous = value; }
	}

	public static UIAnimator AttachTo (GameObject obj) {
		UIAnimator anim = obj.GetComponent<UIAnimator> ();
		if (anim != null)
			return anim;
		return obj.AddComponent<UIAnimator> ();
	}

	public bool Animate (UIAnimation animation, RectTransform rect=null) {
		if (!AllowSimultaneous && Animating)
			return false;
		animation.Rect = rect == null ? gameObject.GetComponent<RectTransform> () : rect;
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

	public class Linear : Curve {
		public override float Get (float x) {
			return Mathf.Lerp (0f, 1f, x);
		}
	}

	/**
	 *	Animations
	 */

	public class Spin : UIAnimation {

		Smooth curve = new Smooth ();

		public Spin (float time, Action onEnd=null) : base (time, (float p) => {
			Rect.SetLocalEulerAnglesZ (curve.Get (p) * 360f);
		}, onEnd) {}
	}

	public class Move : UIAnimation {

		Smooth curve = new Smooth ();
		Vector3 startPosition;

		public Move (float time, Vector3 target, Action onEnd=null) : base (time, (float p) => {
			Rect.SetLocalPosition (Vector3.Lerp (startPosition, target, curve.Get (p)));
		}, onEnd) {}

		protected override void OnLoad () {
			startPosition = Rect.localPosition;
		}
	}

	public class FadeOut : Fade {
		public FadeOut (float time, Action onEnd=null) : base (time, 1f, 0f, onEnd) {}
	}

	public class FadeIn : Fade {
		public FadeIn (float time, Action onEnd=null) : base (time, 0f, 1f, onEnd) {}
	}

	public class Fade : UIAnimation {

		CanvasGroup cg;
		Linear curve = new Linear ();

		public Fade (float time, float from, float to, Action onEnd=null) : base (time, (float p) => {
			cg.alpha = Mathf.Lerp (from, to, curve.Get (p));
		}, onEnd) {}

		protected override void OnLoad () {
			cg = Rect.gameObject.GetComponent<CanvasGroup> ();
			if (cg == null)
				cg = Rect.gameObject.AddComponent<CanvasGroup> ();
		}
	}

	public class Expand : UIAnimation {

		EaseOutBounce curve = new EaseOutBounce ();

		public Expand (float time, Action onEnd=null) : base (time, (float p) => {
			Rect.SetLocalScale (curve.Get (p));
		}, onEnd) {}
	}

	public class Shrink : UIAnimation {

		EaseOutBounce curve = new EaseOutBounce ();

		public Shrink (float time, Action onEnd=null) : base (time, (float p) => {
			Rect.localScale = Vector3.Lerp (Vector3.one, Vector3.zero, curve.Get (p));
		}, onEnd) {}
	}

	public class Peek : UIAnimation {
		
		float startPosition = 0;
		Smooth curve = new Smooth ();

		public Peek (float time, float to, Action onEnd=null) : base (time, (float p) => {
			Rect.SetAnchoredPositionY (Mathf.Lerp (startPosition, to, curve.Get (p)));
		}, onEnd) {}

		protected override void OnLoad () {
			startPosition = Rect.anchoredPosition.y;
		}
	}

	public class Slide : UIAnimation {

		Smooth curve = new Smooth ();

		public Slide (float time, float to, Action onEnd=null) : base (time, (float p) => { 
			Rect.SetAnchoredPositionX (Mathf.Lerp (0f, to, curve.Get (p)));
		}, onEnd) {}
	}

	public abstract class UIAnimation {

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

		public UIAnimation (float time, Action<float> anim, Action onEnd=null) {
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
