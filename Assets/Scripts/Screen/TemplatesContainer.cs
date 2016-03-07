using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Views;

namespace Templates {

	public class TemplatesContainer : UIElement {

		public float slidePosition = 0f; // %

		public Canvas canvas;
		
		public BackButtonElementUI backButton;
		public PotElementUI pot;
		public CoinsElementUI coins;

		TemplateContainer container1;
		TemplateContainer container2;

		float canvasWidth;
		float slideTime = 0.33f;
		bool animating = false;
		string prevId;

		TemplateContainer activeContainer;
		TemplateContainer inactiveContainer;

		GraphicRaycaster raycaster;
		GraphicRaycaster Raycaster {
			get {
				if (raycaster == null) 
					raycaster = canvas.GetComponent<GraphicRaycaster> ();
				return raycaster;
			}
		}

		// Specify overrides for the default transition behaviour (slides in if the new template is listed after the previous one, out otherwise)
		// true = SlideIn
		Dictionary<string, bool> transitionOverrides = new Dictionary<string, bool> () {
			{ "agenda_item_accept agenda_item", true },
			{ "agenda_item_reject agenda_item", true },
			{ "scoreboard roles", true },
			{ "final_scoreboard roles", true },
			{ "pitch extra_time", true },
			{ "extra_time pitch", false }
		};

		public void Load (string id, View view) {

			if (activeContainer == null) {

				// Initialize if this is the first time loading a view
				activeContainer = container1;
				activeContainer.LoadView (id, view);
				inactiveContainer = container2;
				inactiveContainer.RectTransform.SetAnchoredPositionX (canvasWidth);
			} else {
				inactiveContainer.LoadView (id, view);

				// Trigger the slide animation
				// If the new screen and previous screen have a transition override specified, use that
				// Otherwise, use the default behaviour
				bool slideIn;
				if (transitionOverrides.TryGetValue (prevId + " " + id, out slideIn)) {
					if (slideIn) {
						SlideIn ();
					} else {
						SlideOut ();
					}
				} else {
					if (inactiveContainer.TemplateIsBefore (id, prevId)) {
						SlideOut ();
					} else {
						SlideIn ();
					}
				}
			}

			prevId = id;
		}

		void OnEnable () {
			canvasWidth = canvas.GetComponent<RectTransform> ().sizeDelta.x;
			container1 = TemplateContainer.Init (this, 0);
			container2 = TemplateContainer.Init (this, 1);
		}

		void SlideIn () { Slide (-canvasWidth); }
		void SlideOut () { Slide (canvasWidth); }

		void Slide (float to) {

			if (animating) return;
			animating = true;

			// Disable the raycaster while animating so that the user can't "double press" buttons
			Raycaster.enabled = false;

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

				// Enable the raycaster so that input is accepted again
				Raycaster.enabled = true;
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