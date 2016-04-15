using UnityEngine;
using System;
using System.Collections;

public class TemplateAnimator : UIElement {

	TemplateAnimation currentAnimation;

	bool Animating {
		get { return currentAnimation != null && currentAnimation.Animating; }
	}

	public static TemplateAnimator AttachTo (GameObject obj) {
		TemplateAnimator anim = obj.GetComponent<TemplateAnimator> ();
		if (anim != null)
			return anim;
		return obj.AddComponent<TemplateAnimator> ();
	}

	public bool Animate (TemplateAnimation animation) {
		if (Animating)
			return false;
		animation.Rect = gameObject.GetComponent<RectTransform> ();
		currentAnimation = animation;
		animation.Start ();
		return true;
	}

	public class Peek : TemplateAnimation {
		
		float startPosition = 0;

		public Peek (float time, float to, Action onEnd=null) : base (time, (float p) => {
			Rect.SetAnchoredPositionY (Mathf.Lerp (startPosition, to, Mathf.SmoothStep (0f, 1f, p)));
		}, onEnd) {}

		protected override void OnLoad () {
			startPosition = Rect.anchoredPosition.y;
		}
	}

	public class Slide : TemplateAnimation {
		public Slide (float time, float to, Action onEnd=null) : base (time, (float p) => { 
			Rect.SetAnchoredPositionX (Mathf.Lerp (0f, to, Mathf.SmoothStep (0f, 1f, p))); 
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

		public TemplateAnimation (float time, Action<float> anim, Action onEnd) {
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
