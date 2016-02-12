using UnityEngine;
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

	Dictionary<string, ScreenElement> elements;
	public Dictionary<string, ScreenElement> Elements {
		get {
			if (elements == null) {
				elements = new Dictionary<string, ScreenElement> ();
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

	public void Init (GameScreenManager screens, Transform canvas) {
		this.screens = screens;
		this.canvas = canvas;
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
	}

	void Render () {
		foreach (var element in Elements) {
			element.Value.Render (this).SetParent (canvas);
		}
	}

	void RenderDynamic () {
		foreach (var element in dynamicElements) {
			ScreenElement s = element.Value;
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
			for (int i = 0; i < Role.AgendaItems.Length; i ++) {
				Elements.Add ("rc_agenda" + i.ToString (), new TextElement (Role.AgendaItems[i].Description));
				Elements.Add ("rc_reward" + i.ToString (), new TextElement ("Reward: " + Role.AgendaItems[i].Reward));
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
