using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Models;

namespace Views {

	/// <summary>
	/// Handles content that gets displayed on the screen
	///
	/// A "View" pulls in content from the API (or loads it locally)
	/// 	and then determines which content should be rendered on the screen.
	///		Better practice is to keep logic to a minimum.
	///
	/// This is the rendering order:
	/// 	1. Elements set by the API (check Models.Screen), with the exception of Buttons and Text
	///		2. Elements that all players see (override OnInitElements)
	///		3. Elements that only the Decider sees (override OnInitDeciderElements)
	///		4. Elements that all players except the Decider see (override OnInitPlayerElements)
	///
	/// </summary>
	public abstract class View : GameInstanceComponent {

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
			get { return Game.Name; }
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
			get { return Game.Controller.Role; }
		}

		/// <summary>
		/// Returns the model associated with this player
		/// </summary>
		protected Models.Screen Model { get; private set; }

		/// <summary>
		/// Gets the amount of coins that the Decider starts with in this round
		/// </summary>
		protected int DeciderRoundStartCoinCount {
			get {
				return Game.Controller.RoundNumber == 0
					? Settings.DeciderStartCoinCount
					: Settings.DeciderRoundStartCoinCount;
			}
		}

		/// <summary>
		/// Gets the amount of coins that players start with in this round
		/// </summary>
		protected int PlayerRoundStartCoinCount {
			get {
				return Game.Controller.RoundNumber == 0
					? Settings.PlayerStartCoinCount
					: Settings.PlayerRoundStartCoinCount;
			}
		}

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
					if (IsHost) {
						OnInitHostElements ();
					} else {
						OnInitClientElements ();
					}
					InitElements ();
					OnShow ();
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

				Dictionary<string, string> textVars = new Dictionary<string, string> () {
					{ "decider", Game.Controller.DeciderName },
					{ "decider_start_coin_count", DeciderRoundStartCoinCount.ToString () },
					{ "player_start_coin_count", PlayerRoundStartCoinCount.ToString () },
					{ "pot_coin_count", Settings.PotCoinCount.ToString () },
					{ "extra_time_cost", Settings.ExtraTimeCost.ToString () },
					{ "extra_seconds", Settings.ExtraSeconds.ToString () },
					{ "round_number", (Game.Controller.RoundNumber+1).ToString () },
					{ "winner", Game.Controller.WinnerName },
					{ "question", Game.Controller.Question },
					{ "player_name", Name },
					{ "think_seconds", Settings.ThinkSeconds.ToString () },
					{ "pitch_seconds", Settings.PitchSeconds.ToString () },
					{ "deliberate_seconds", Settings.DeliberateSeconds.ToString () }
				};

				return textVars;
			}
		}

		/// <summary>
		/// Settings, grabbed from DataManager
		/// </summary>
		Settings settings;
		protected Settings Settings {
			get {
				if (settings == null)
					settings = DataManager.GetSettings ();
				return settings;
			}
		}

		/// <summary>
		/// Optionally override this method to pass additional data to the template
		/// </summary>
		public virtual ViewData Data {
			get { return new ViewData (); }
		}

		protected ViewManager views;

		/// <summary>
		/// Initializes the screen. This should only ever be called by ViewManager
		/// </summary>
		public void Init (ViewManager views, string id) {
			this.views = views;
			Model = DataManager.GetScreen (id);
			Init (views);
		}

		/// <summary>
		/// Hides the screen. This should only ever be called by ViewManager
		/// </summary>
		public void Hide () {
			OnHide ();
			elements = null;
		}

		/// <summary>
		/// Gets the screen element with the id as the type T
		/// </summary>
		/// <param name="id">The id of the screen element</param>
		public T GetScreenElement<T> (string id) where T : ScreenElement {
			try {
				return Elements[id] as T;
			} catch (KeyNotFoundException) {
				throw new System.Exception ("No screen element with the id '" + id + "'");
			}
		}

		void LoadModelElements () {

			// Header/title
			if (!string.IsNullOrEmpty (Model.DisplayName)) {
				Elements.Add ("title", new TextElement (Model.DisplayName));
			}

			// Decider & Player instructions
			if (IsDecider) {
				if (!string.IsNullOrEmpty (Model.DeciderInstructionsOutLoud)) {
					Elements.Add ("decider_instructions_out_loud", new TextElement (
						DataManager.GetTextFromScreen (Model, "DeciderInstructionsOutLoud", TextVariables)));
					Elements.Add ("instructions_read", new TextElement ("Read out loud:"));
				}
				if (!string.IsNullOrEmpty (Model.DeciderInstructions)) {
					Elements.Add ("decider_instructions", new TextElement (
						DataManager.GetTextFromScreen (Model, "DeciderInstructions", TextVariables)));
				}
			} else {
				if (!string.IsNullOrEmpty (Model.PlayerInstructions)) {
					Elements.Add ("player_instructions", new TextElement (
						DataManager.GetTextFromScreen (Model, "PlayerInstructions", TextVariables)));
				}
			}

			// Host & Client instructions
			if (IsHost) {
				if (!string.IsNullOrEmpty (Model.HostInstructions)) {
					Elements.Add ("host_instructions", new TextElement (
						DataManager.GetTextFromScreen (Model, "HostInstructions", TextVariables)));
				}
			} else {
				if (!string.IsNullOrEmpty (Model.ClientInstructions)) {
					Elements.Add ("client_instructions", new TextElement (
						DataManager.GetTextFromScreen (Model, "ClientInstructions", TextVariables)));
				}
			}

			// Everyone instructions
			if (!string.IsNullOrEmpty (Model.Instructions)) {
				Elements.Add ("instructions", new TextElement (
					DataManager.GetTextFromScreen (Model, "Instructions", TextVariables)));
			}

			Elements.Add ("pot", new PotElement ());
			Elements.Add ("coins", new CoinsElement ());
		}

		void InitElements () {
			foreach (var element in Elements) {
				element.Value.Init (Behaviour);
			}
		}

		protected void CreateRoleCard (bool showTitle, bool showBio, bool showAgenda, bool showAgendaImages=true) {

			// The title ("Bob the Builder")
			if (showTitle) Elements.Add ("rc_title", new TextElement (Name + " the " + Title));

			// The character description ("Construction worker who makes beautiful buildings")
			if (showBio) Elements.Add ("rc_bio", new TextElement (Role.Bio));

			if (showAgenda) {

				int[] rewardValues = DataManager.GetSettings ().Rewards;

				// The Agenda caption ("Agenda")
				Elements.Add ("rc_agenda_title", new TextElement ("Agenda"));

				for (int i = 0; i < Role.AgendaItems.Length; i ++) {

					string idx = i.ToString ();

					// Agenda item description ("Includes dump trucks")
					Elements.Add ("rc_item" + idx, new TextElement (Role.AgendaItems[i].Description));

					// Agenda item values (coin image with value)
					if (showAgendaImages) {
						Elements.Add ("rc_reward_image" + idx, new ImageElement ("coin"));
						Elements.Add ("rc_reward" + idx, new TextElement ("+" + rewardValues[Role.AgendaItems[i].Reward]));
						if (i > 0) {
							Elements.Add ("rc_reward_image" + idx + "b", new ImageElement ("coin"));
						}
					}
				}
			}
		}

		protected bool HasElement (string id) {
			return Elements.ContainsKey (id);
		}

		protected string GetText (string id) {
			return DataManager.GetTextFromScreen (Model, id, TextVariables);
		}

		protected string GetButton (string id) {
			try {
				return Model.Buttons[id];
			} catch (KeyNotFoundException e) {
				throw new System.Exception ("Could not find data for a button with the id '" + id + "'\n" + e);
			}
		}

		// -- Routing

		/// <summary>
		/// Moves the player to a new view
		/// </summary>
		/// <param name="id">The id of the view to move to</param>
		public void GotoView (string id) {
			views.Goto (id);
		}

		/// <summary>
		/// Moves the player back to the previous view
		/// </summary>
		public void GoBack () {
			views.GotoPrevious ();
		}

		/// <summary>
		/// Sends all connected players to a new view
		/// </summary>
		/// <param name="id">The id of the view to move to</param>
		public void AllGotoView (string id) {
			views.AllGoto (id);
		}

		// -- Events

		/// <summary>
		/// Called when the player is disconnected. By default, the player is sent to the "disconnected" view, but views can override this and define custom behavior
		/// </summary>
		public virtual void OnDisconnect () {
			GotoView ("disconnected");
		}

		/// <summary>
		/// Called when a player (other than this one) is disconnected from the game. By default, the player is sent to the "dropped" view, but views can override this and define custom behavior
		/// </summary>
		public virtual void OnClientDropped () {
			GotoView ("dropped");
		}

		/// <summary>
		/// Called when a previously disconnected player rejoins the game. By default, the player is sent to the previously visited view, but views can override this and define custom behavior
		/// </summary>
		public virtual void OnClientsReconnected () {
			views.GotoViewBeforeDrop ();
		}

		// -- Misc

		/// <summary>
		/// Request to join a game with the given host id and handle the response
		/// </summary>
		/// <param name="hostId">The host id of the game to join</param>
		protected void JoinGame (string hostId) {
			Game.StartGame ();
			Game.Multiplayer.JoinGame (hostId, (ResponseType res) => {
				switch (res) {
					case ResponseType.Success: GotoView ("lobby"); break;
					case ResponseType.NameTaken:
						Game.Multiplayer.TakenName = Name;
						Game.Multiplayer.AttemptedHost = hostId;
						Game.EndGame ();
						OnNameTaken ();
						break;
				}
			});
		}

		/// <summary>
		/// Request to host a new game and handle the response
		/// </summary>
		protected void HostGame () {
			Game.StartGame ();
			Game.Multiplayer.HostGame ((ResponseType res) => {
				
				if (res == ResponseType.Success) {
					GotoView ("lobby");
				} else if (res == ResponseType.NameTaken) {
					Game.Multiplayer.TakenName = Name;
					Game.Multiplayer.AttemptedHost = "";
					Game.EndGame ();
					OnNameTaken ();
				}
			});
		}

		/// <summary>
		/// On a screen with timers, checks how much time has elapsed
		/// </summary>
		/// <returns>The amount of time that has elapsed</returns>
		protected int GetElapsedTime () {

			float e;

			if (IsDecider) {
				TimerButtonElement t = GetScreenElement<TimerButtonElement> ("timer_button");
				e = t.Progress * t.Duration;
			} else {
				TimerElement t = GetScreenElement<TimerElement> ("timer");
				e = t.Progress * t.Duration;
			}

			return (int)e;
		}

		// -- Virtual methods

		/// <summary>
		/// Static elements that all players see
		/// </summary>
		protected virtual void OnInitElements () {}

		/// <summary>
		/// Static elements that only the Decider sees
		/// </summary>
		protected virtual void OnInitDeciderElements () {}
		
		/// <summary>
		/// Static elements that all players except the Decider see
		/// </summary>
		protected virtual void OnInitPlayerElements () {}

		/// <summary>
		/// Static elements that only the host sees
		/// </summary>
		protected virtual void OnInitHostElements () {}

		/// <summary>
		/// Static elements that only clients seee
		/// </summary>
		protected virtual void OnInitClientElements () {}

		/// <summary>
		/// Called after all screen elements have been loaded
		/// </summary>
		protected virtual void OnShow () {}

		/// <summary>
		/// Called when the player leaves the view
		/// </summary>
		protected virtual void OnHide () {}

		/// <summary>
		/// Called when the player attempted to join or host a game but their name is unavailable
		/// </summary>
		protected virtual void OnNameTaken () {}
	}
}