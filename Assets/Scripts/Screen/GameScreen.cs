﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Models;

public abstract class GameScreen : GameInstanceComponent {

	protected bool IsHost {
		get { return Game.Multiplayer.Hosting; }
	}

	protected bool IsDecider {
		get { return Role != null && Role.Title == "Decider"; }
	}

	protected string Name {
		get { return Game.Manager.Player.Name; }
	}

	protected string Title {
		get { return Role.Title; }
	}

	protected Role Role {
		get { return Game.Manager.Player.Role; }
	}

	protected Models.Screen Model { get; private set; }

	Dictionary<string, ScreenElement> elements;
	public Dictionary<string, ScreenElement> Elements {
		get {
			if (elements == null) {
				elements = new Dictionary<string, ScreenElement> ();
				LoadModelElements ();
				OnInitElements ();
				if (IsDecider) {
					OnInitDeciderElements ();
				} else {
					OnInitPlayerElements ();
				}
			}
			return elements;
		}
	}

	Dictionary<string, ScreenElement> dynamicElements = new Dictionary<string, ScreenElement> ();

	Transform canvas;
	protected GameScreenManager screens;

	public void Init (GameScreenManager screens, Transform canvas, string id) {
		this.screens = screens;
		this.canvas = canvas;
		Model = DataManager.GetScreen (id);
		Init (screens);
	}
	
	public void Show () {
		Render ();
		OnShow ();
	}

	public void Hide () {
		foreach (var element in Elements) {
			element.Value.Remove ();
		}
		foreach (var element in dynamicElements) {
			element.Value.Remove ();
		}
		dynamicElements.Clear ();
		OnHide ();
		elements = null;
	}

	void LoadModelElements () {
		if (Model != null) { // TODO: remove this check once all models are in
			if (!string.IsNullOrEmpty (Model.DisplayName)) {
				Elements.Add ("title", new TextElement (Model.DisplayName));
			}
			if (IsDecider) {
				if (!string.IsNullOrEmpty (Model.DeciderInstructions)) {
					Elements.Add ("decider_instructions", new TextElement (Model.DeciderInstructions));
				}
			}
			if (IsHost) {
				if (!string.IsNullOrEmpty (Model.HostInstructions)) {
					Elements.Add ("host_instructions", new TextElement (Model.HostInstructions));
				}
			} 
			if (!IsDecider && !IsHost) {
				if (!string.IsNullOrEmpty (Model.Instructions)) {
					Elements.Add ("instructions", new TextElement (Model.Instructions));
				}
			}
			if (Model.DisplayScore) {
				Elements.Add ("pot", new PotElement ());
				Elements.Add ("coins", new CoinsElement ());
			}
		}
	}

	void Render () {
		foreach (var element in Elements) {
			element.Value.Init (Behaviour);
			element.Value.Render (this).SetParent (canvas);
		}
	}

	void RenderDynamic () {
		foreach (var element in dynamicElements) {
			ScreenElement s = element.Value;
			s.Init (Behaviour);
			if (!s.Active)
				s.Render (this).SetParent (canvas);
		}
	}

	protected T GetScreenElement<T> (string id) where T : ScreenElement {
		return Elements[id] as T;
	}

	protected void CreateRoleCard (bool showTitle, bool showBio, bool showAgenda) {
		if (showTitle) Elements.Add ("rc_title", new TextElement (Name + " the " + Title));
		if (showBio) Elements.Add ("rc_bio", new TextElement (Role.Bio));
		if (showAgenda) {
			int[] rewardValues = DataManager.GetSettings ().Rewards;
			for (int i = 0; i < Role.AgendaItems.Length; i ++) {
				Elements.Add ("rc_agenda" + i.ToString (), new TextElement (Role.AgendaItems[i].Description));
				Elements.Add ("rc_reward" + i.ToString (), new TextElement ("Reward: " + rewardValues[Role.AgendaItems[i].Reward]));
			}
		}
	}

	// Dynamic elements
	protected T AddElement<T> (string id, T t) where T : ScreenElement {
		try {
			dynamicElements.Add (id, t);
		} catch {
			throw new System.Exception ("An element with the id '" + id + "' already exists on the screen.");
		}
		RenderDynamic ();
		return t;
	}

	protected void AddElement (string id, ScreenElement element) {
		dynamicElements.Add (id, element);
		RenderDynamic ();
	}

	protected void RemoveElement (string id) {
		dynamicElements[id].Remove ();
		dynamicElements.Remove (id);
		RenderDynamic ();
	}

	protected bool HasElement (string id) {
		return dynamicElements.ContainsKey (id);
	}

	// Routing
	public void GotoScreen (string id) {
		screens.SetScreen (id);
	}

	public void GoBack () {
		screens.SetScreenPrevious ();
	}

	public void AllGotoScreen (string id) {
		Game.Dispatcher.ScheduleMessage ("GotoScreen", id);
	}

	// Events
	protected virtual void OnInitElements () {}			// Static elements that all players see
	protected virtual void OnInitDeciderElements () {}	// Static elements that only the Decider sees
	protected virtual void OnInitPlayerElements () {}	// Static elements that all players except the Decider see
	public virtual void OnDisconnect () {}
	protected virtual void OnShow () {}
	protected virtual void OnHide () {}
}
