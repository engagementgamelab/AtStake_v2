using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Models;

/// <summary>
/// Handles content that gets displayed on the screen
///
/// A "GameScreen" can be thought of as a "view" - it pulls in content from the API (or loads it locally)
/// 	and then determines which content should be rendered on the screen.
///		Better practice is to keep logic to a minimum.
///
/// This is the rendering order:
/// 	1. Elements set by the API (check Models.Screen), with the exception of Buttons and Text
///		2. Elements that all players see (override OnInitElements)
///		3. Elements that only the Decider sees (override OnInitDeciderElements)
///		4. Elements that all players except the Decider see (override OnInitPlayerElements)
///
/// After the screen has loaded you can add additional elements by calling AddElement()
/// </summary>
public abstract class GameScreen : GameInstanceComponent {

	/// <summary>
	/// Returns true if this player is hosting
	/// </summary>
	protected bool IsHost {
		get { return Game.Multiplayer.Hosting; }
	}

	/// <summary>
	/// Returns true if this player is the Decider
	/// </summary>
	protected bool IsDecider {
		get { return Role != null && Role.Title == "Decider"; }
	}

	/// <summary>
	/// Returns this player's name
	/// </summary>
	protected string Name {
		get { return Game.Manager.Player.Name; }
	}

	/// <summary>
	/// Returns this player's role title
	/// </summary>
	protected string Title {
		get { return Role.Title; }
	}

	/// <summary>
	/// Returns this player's role
	/// </summary>
	protected Role Role {
		get { return Game.Manager.Player.Role; }
	}

	/// <summary>
	/// Returns the model associated with this player
	/// </summary>
	protected Models.Screen Model { get; private set; }

	/// <summary>
	/// The static elements that get rendered on the screen
	/// These are initialized when they're first referenced
	/// Generally you do not need to work directly with this dictionary
	/// </summary>
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

	/// <summary>
	/// Some text pulled in from the API contains variables surrounded by {{double_brackets}}
	/// This lookup is used to replace those variables with the associated value
	/// </summary>
	protected Dictionary<string, string> TextVariables {
		get {
			return new Dictionary<string, string> () {
				{ "decider", Game.Manager.Decider },
				{ "decider_start_coin_count", Settings.DeciderStartCoinCount.ToString () },
				{ "player_start_coin_count", Settings.PlayerStartCoinCount.ToString () },
				{ "pot_coin_count", Settings.PotCoinCount.ToString () },
				{ "extra_time_cost", Settings.ExtraTimeCost.ToString () },
				{ "extra_seconds", Settings.ExtraSeconds.ToString () },
				{ "round_number", (Game.Rounds.Current+1).ToString () },
				{ "winner", Game.Manager.Winner }
			};
		}
	}

	Dictionary<string, ScreenElement> dynamicElements = new Dictionary<string, ScreenElement> ();

	Settings settings;
	Settings Settings {
		get {
			if (settings == null)
				settings = DataManager.GetSettings ();
			return settings;
		}
	}

	Transform canvas;
	protected GameScreenManager screens;

	/// <summary>
	/// Initializes the screen. This should only ever be called by GameScreenManager
	/// </summary>
	public void Init (GameScreenManager screens, Transform canvas, string id) {
		this.screens = screens;
		this.canvas = canvas;
		Model = DataManager.GetScreen (id);
		Init (screens);
	}
	
	/// <summary>
	/// Shows the screen. This should only ever be called by GameScreenManager
	/// </summary>
	public void Show () {
		Render ();
		OnShow ();
	}

	/// <summary>
	/// Hides the screen. This should only ever be called by GameScreenManager
	/// </summary>
	public void Hide () {
		/*foreach (var element in Elements) {
			element.Value.Remove ();
		}*/
		Game.Ui.RemoveElements (elements);
		foreach (var element in dynamicElements) {
			element.Value.Remove ();
		}
		dynamicElements.Clear ();
		OnHide ();
		elements = null;
	}

	void LoadModelElements () {
		if (!string.IsNullOrEmpty (Model.DisplayName)) {
			Elements.Add ("title", new TextElement (Model.DisplayName, TextStyle.Header));
		}
		if (IsDecider) {
			if (!string.IsNullOrEmpty (Model.DeciderInstructions)) {
				Elements.Add ("decider_instructions", new TextElement (
					DataManager.GetTextFromScreen (Model, "DeciderInstructions", TextVariables)));
			}
		}
		if (IsHost) {
			if (!string.IsNullOrEmpty (Model.HostInstructions)) {
				Elements.Add ("host_instructions", new TextElement (
					DataManager.GetTextFromScreen (Model, "HostInstructions", TextVariables)));
			}
		} 
		if (!IsDecider) {
			if (!string.IsNullOrEmpty (Model.Instructions)) {
				Elements.Add ("instructions", new TextElement (
					DataManager.GetTextFromScreen (Model, "Instructions", TextVariables)));
			}
		}
		if (Model.DisplayScore) {
			Elements.Add ("pot", new PotElement ());
			Elements.Add ("coins", new CoinsElement ());
		}
	}

	void Render () {
		foreach (var element in Elements) {
			element.Value.Init (Behaviour, this);
			// element.Value.Render (this).SetParent (canvas);
		}
		Game.Ui.RenderElements (elements);
	}

	void RenderDynamic () {
		foreach (var element in dynamicElements) {
			ScreenElement s = element.Value;
			s.Init (Behaviour, this);
			/*if (!s.Active)
				s.Render (this).SetParent (canvas);*/
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
