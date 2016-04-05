using UnityEngine;
using System.Collections;
using Views;

public class GameTest : GameInstanceBehaviour {

	bool Animating {
		get { return Game.Templates.templatesContainer.Animating; }
	}

	bool Hosting {
		get { return Game.Multiplayer.Hosting; }
	}

	bool IsDecider {
		get { return Game.Controller.Role.Title == "Decider"; }
	}

	View CurrentView {
		get { return Game.Views.GetView (); }
	}

	public void Init () {
		Game.Dispatcher.AddListener ("RunTest", Run);
	}

	void Run (MasterMsgTypes.GenericMessage msg) {

		Game.Dispatcher.AddListener ("GotoView", GotoView);

		if (!Hosting)
			return;

		/*if (!Hosting) {
			Debug.LogWarning ("Only the host can start the test");
			return;
		}*/

		if (Game.Views.CurrView != "lobby") {
			Debug.LogWarning ("Test must begin in the lobby");
			return;
		}

		PressButton ("play");
	}

	public void PressNext () {
		if (IsDecider)
			PressButton ("next");
	}

	public void PressButton (string id) {
		CurrentView.GetScreenElement<ButtonElement> (id).OnPress ();
	}

	public void PressRadioButton (string radioId, string id) {
		RadioListElement list = CurrentView.GetScreenElement<RadioListElement> (radioId);
		list.Elements[id].OnPressThis (list.Elements[id]);
		list.Elements["confirm"].OnPress ();
	}

	public void PressTimerButton () {
		CurrentView.GetScreenElement<TimerButtonElement> ("timer_button").StartTimer ();
	}

	void GotoView (MasterMsgTypes.GenericMessage msg) {

		Co.YieldWhileTrue (() => { return Animating; }, () => {
			switch (msg.str1) {
				case "deck": 
					if (Hosting)
						PressRadioButton ("deck_list", "Default");
					break;
				case "roles":
				case "pot":
				case "bio":
				case "agenda":
				case "question":
				case "think_instructions":
				case "pitch_instructions":
				case "deliberate_instructions":
					PressNext ();
					break;
				case "think":
					if (IsDecider)
						PressTimerButton ();
					break;
				case "pitch":
					if (IsDecider) {
						PressTimerButton ();
					} else {
						/*Co.YieldWhileTrue (() => { return Game.Views.CurrView != "extra_time"; }, () => {

							GotoView (MasterMsgTypes.GenericMessage.Create ("GotoView", "extra_time"));
						});*/
					}
					break;
				case "extra_time":
					PressButton ("decline");
					break;
			}
		});
	}
}
