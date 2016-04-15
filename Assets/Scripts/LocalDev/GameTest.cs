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
		CurrentView.GetScreenElement<ButtonElement> (id).TestPress ();
	}

	public void PressRadioButton (string radioId, string id) {
		RadioListElement list = CurrentView.GetScreenElement<RadioListElement> (radioId);
		list.Elements[id].TestPress ();
		list.Elements["confirm"].TestPress ();
	}

	public void PressListButton (string listId, string id) {
		ListElement<ButtonElement> list = CurrentView.GetScreenElement<ListElement<ButtonElement>> (listId);
		list.Elements[id].TestPress ();
	}

	public void PressTimerButton () {
		CurrentView.GetScreenElement<TimerButtonElement> ("timer_button").StartTimer ();
	}

	void GotoView (MasterMsgTypes.GenericMessage msg) {

		Co.YieldWhileTrue (() => { return Animating; }, () => {
			Co.WaitForSeconds (1f, () => {
				switch (msg.str1) {
					case "deck": 
						if (Hosting)
							PressRadioButton ("deck_list", "Default");
						break;
					case "pot":
					case "bio":
					case "agenda":
					case "question":
					case "think_instructions":
					case "pitch_instructions":
					case "deliberate_instructions":
					case "winner":
					case "agenda_item_accept":
					case "agenda_item_reject":
					case "scoreboard":
						PressNext ();
						break;
					case "roles":
						if (IsDecider)
							PressButton ("play");
						break;
					case "think":
						if (IsDecider)
							PressTimerButton ();
						break;
					case "pitch":
						if (IsDecider) {
							PressTimerButton ();
						} else {
							Co.YieldWhileTrue (() => { return Game.Views.CurrView != "extra_time"; }, () => {
								GotoView (MasterMsgTypes.GenericMessage.Create ("GotoView", "extra_time"));
							});
						}
						break;
					case "extra_time":
					case "extra_time_deliberate":
						PressButton ("decline");
						break;
					case "deliberate":
						if (IsDecider) {
							PressTimerButton ();
						} else {
							Co.YieldWhileTrue (() => { return Game.Views.CurrView != "extra_time_deliberate"; }, () => {
								GotoView (MasterMsgTypes.GenericMessage.Create ("GotoView", "extra_time_deliberate"));
							});
						}
						break;
					case "decide":
						if (IsDecider) {
							PressListButton ("peer_list", Game.Controller.PeerNames[0]);
							PressButton ("confirm");
						}
						break;
					case "agenda_item":
						if (IsDecider) {
							bool accept = Random.value > 0.5f;
							if (accept) {
								PressButton ("accept");
							} else {
								PressButton ("reject");
							}
						}
						break;
				}
			});
		});
	}
}
