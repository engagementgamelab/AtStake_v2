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
				return new Dictionary<string, string> () {
					{ "decider", Game.Controller.DeciderName },
					{ "decider_start_coin_count", Settings.DeciderStartCoinCount.ToString () },
					{ "player_start_coin_count", Settings.PlayerStartCoinCount.ToString () },
					{ "pot_coin_count", Settings.PotCoinCount.ToString () },
					{ "extra_time_cost", Settings.ExtraTimeCost.ToString () },
					{ "extra_seconds", Settings.ExtraSeconds.ToString () },
					{ "round_number", Game.Controller.RoundNumber.ToString () },
					{ "winner", Game.Controller.WinnerName }
				};
			}
		}

		Settings settings;
		Settings Settings {
			get {
				if (settings == null)
					settings = DataManager.GetSettings ();
				return settings;
			}
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

		void LoadModelElements () {

			// Header/title
			if (!string.IsNullOrEmpty (Model.DisplayName)) {
				Elements.Add ("title", new TextElement (Model.DisplayName, TextStyle.Header));
			}

			// Decider & Player instructions
			if (IsDecider) {
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

		protected T GetScreenElement<T> (string id) where T : ScreenElement {
			try {
				return Elements[id] as T;
			} catch (KeyNotFoundException) {
				throw new System.Exception ("No screen element with the id '" + id + "'");
			}
		}

		protected void CreateRoleCard (bool showTitle, bool showBio, bool showAgenda) {
			if (showTitle) Elements.Add ("rc_title", new TextElement (Name + " the " + Title));
			if (showBio) Elements.Add ("rc_bio", new TextElement (Role.Bio));
			if (showAgenda) {
				Dictionary<string, TextElement> agendaItems = new Dictionary<string, TextElement> ();
				int[] rewardValues = DataManager.GetSettings ().Rewards;
				for (int i = 0; i < Role.AgendaItems.Length; i ++) {
					agendaItems.Add ("rc_item" + i.ToString (), new TextElement (Role.AgendaItems[i].Description));
					agendaItems.Add ("rc_reward" + i.ToString (), new TextElement ("Reward: " + rewardValues[Role.AgendaItems[i].Reward]));
				}
				Elements.Add ("rc_agenda", new ListElement<TextElement> (agendaItems));
			}
		}

		protected bool HasElement (string id) {
			return Elements.ContainsKey (id);
		}

		protected string GetText (string id) {
			return DataManager.GetTextFromScreen (Model, id, TextVariables);
		}

		// Routing
		public void GotoView (string id) {
			views.Goto (id);
		}

		public void GoBack () {
			views.GotoPrevious ();
		}

		public void AllGotoView (string id) {
			views.AllGoto (id);
		}

		// Events
		protected virtual void OnInitElements () {}			// Static elements that all players see
		protected virtual void OnInitDeciderElements () {}	// Static elements that only the Decider sees
		protected virtual void OnInitPlayerElements () {}	// Static elements that all players except the Decider see
		protected virtual void OnInitHostElements () {}		// Static elements that only the host sees
		protected virtual void OnInitClientElements () {}	// Static elements that only clients seee
		public virtual void OnDisconnect () {}
		protected virtual void OnShow () {}
		protected virtual void OnHide () {}
	}
}