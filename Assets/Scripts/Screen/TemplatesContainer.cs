using UnityEngine;
using System.Collections;
using Views;

namespace Templates {

	public class TemplatesContainer : UIElement {

		public float slidePosition = 0f; // %

		public Canvas canvas;
		public TemplateContainer container1;
		public TemplateContainer container2;

		float canvasWidth;
		float slideTime = 0.33f;
		bool animating = false;
		string prevId;

		TemplateContainer activeContainer;
		TemplateContainer inactiveContainer;

		public void Load (string id, View view) {

			if (activeContainer == null) {

				// Initialize if this is the first time loading a view
				activeContainer = container1;
				activeContainer.LoadView (id, view);
				inactiveContainer = container2;
				inactiveContainer.RectTransform.SetAnchoredPositionX (canvasWidth);
			} else {
				inactiveContainer.LoadView (id, view);
				if (inactiveContainer.TemplateIsBefore (id, prevId)) {
					SlideOut ();
				} else {
					SlideIn ();
				}
			}

			prevId = id;
		}

		void OnEnable () {
			canvasWidth = canvas.GetComponent<RectTransform> ().sizeDelta.x;
		}

		void SlideIn () { Slide (-canvasWidth); }
		void SlideOut () { Slide (canvasWidth); }

		void Slide (float to) {

			if (animating) return;
			animating = true;

			inactiveContainer.RectTransform.SetAnchoredPositionX (-to);

			Co.StartCoroutine (slideTime, (float p) => {
				RectTransform.SetAnchoredPositionX (Mathf.Lerp (0f, to, Mathf.SmoothStep (0f, 1f, p)));
			}, () => {

				// Normalize positions
				inactiveContainer.RectTransform.SetAnchoredPositionX (0f);
				activeContainer.RectTransform.SetAnchoredPositionX (-to);
				RectTransform.SetAnchoredPositionX (0f);

				// Unload the previous view
				activeContainer.UnloadView ();

				// Swap the active and inactive containers
				UpdateActiveContainer ();
				animating = false;
			});
		}

		void UpdateActiveContainer () {
			TemplateContainer temp = activeContainer;
			activeContainer = inactiveContainer;
			inactiveContainer = temp;
		}
	}
}