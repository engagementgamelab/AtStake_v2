using UnityEngine;
using System.Collections;

namespace Templates {

	public class TemplatesContainer : UIElement {

		public float slidePosition = 0f; // %

		public Canvas canvas;
		public TemplateContainer container1;
		public TemplateContainer container2;

		int slideInHash = Animator.StringToHash ("SlideIn");
		int slideOutHash = Animator.StringToHash ("SlideOut");
		int slideIdleHash = Animator.StringToHash ("SlideIdle");

		float canvasWidth;

		void OnEnable () {
			canvasWidth = canvas.GetComponent<RectTransform> ().sizeDelta.x;
			container2.RectTransform.SetAnchoredPositionX (canvasWidth);
		}

		void Update () {
			if (Input.GetKeyDown (KeyCode.E)) {
				AnimateSlide ();
			}
			if (Input.GetKeyDown (KeyCode.R)) {
				Animator.Play (slideOutHash);
			}
			RectTransform.SetAnchoredPositionX (Mathf.Lerp (0f, -canvasWidth, slidePosition));
		}

		void AnimateSlide () {
			Animator.Play (slideIdleHash);
			Co.WaitForFixedUpdate (() => {
				Animator.Play (slideInHash);
			});
		}
	}
}