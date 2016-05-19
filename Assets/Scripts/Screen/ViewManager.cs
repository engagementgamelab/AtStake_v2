using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	//// <summary>
	/// Manages all the views in the game
	/// Sets which view is currently active and moves between views.
	/// </summary>
	public class ViewManager : GameInstanceBehaviour {

		/// <summary>
		/// The id of the view the player is currently on
		/// </summary>
		public string CurrView { get; private set; }

		/// <summary>
		/// The id of the previously visited view
		/// </summary>
		public string PrevView { get; private set; }

		Dictionary<string, View> views;
		Dictionary<string, View> Views {
			get {
				if (views == null) {

					views = new Dictionary<string, View> () {
						{ "start", new Start () },
						{ "name_taken", new NameTaken () },
						{ "hostjoin", new HostJoin () },
						{ "lobby", new Lobby () },
						{ "games", new Games () },
						{ "deck", new Deck () },
						{ "roles", new Roles () },
						{ "pot", new Pot () },
						{ "bio", new Bio () },
						{ "agenda", new Agenda () },
						{ "question", new Question () },
						{ "think_instructions", new ThinkInstructions () },
						{ "think", new Think () },
						{ "pitch_instructions", new PitchInstructions () },
						{ "pitch", new Pitch () },
						{ "extra_time", new ExtraTime () },
						{ "deliberate_instructions", new DeliberateInstructions () },
						{ "deliberate", new Deliberate () },
						{ "extra_time_deliberate", new ExtraTimeDeliberate () },
						{ "decide", new Decide () },
						{ "winner", new Winner () },
						{ "agenda_item_instructions", new AgendaItemInstructions () },
						{ "agenda_item", new AgendaItem () },
						{ "agenda_item_accept", new AgendaItemAccept () },
						{ "agenda_item_reject", new AgendaItemReject () },
						{ "scoreboard", new Scoreboard () },
						{ "final_scoreboard", new FinalScoreboard () },
						{ "disconnected", new Disconnected () },
						{ "dropped", new Dropped () },
						{ "socket_disconnected", new SocketDisconnected () }
					};
				}

				return views;
			}
		}

		bool Loaded { get { return views != null; } }

		string viewBeforeDrop = "";

		public void Init () {
			if (!Loaded) {
				foreach (var view in Views)
					view.Value.Init (this, view.Key);
				Goto ("start");
			}
			Game.Dispatcher.AddListener ("GotoView", OnGotoView);
		}

		/// <summary>
		/// Move to the view with the given id
		/// </summary>
		/// <param name="id">The id of the view to move to</param>
		public void Goto (string id) {

			if (id == CurrView) return;

			// Unload the current view and update the PrevView reference
			if (!string.IsNullOrEmpty (CurrView)) {
				Views[CurrView].Hide ();
				PrevView = CurrView;
			}
			CurrView = id;
			Game.Controller.SetView (CurrView);

			// Render the new view
			if (Views.ContainsKey (CurrView)) {
				Game.Templates.Load (CurrView, Views[CurrView]);
			} else {
				throw new System.Exception ("No template with the id '" + id + "' exists.");
			}
		}

		/// <summary>
		/// Moves the player to the previously visited view
		/// </summary>
		public void GotoPrevious () {
			Goto (PrevView);
		}

		/// <summary>
		/// Return to the view the player was on before a client was dropped
		/// </summary>
		public void GotoViewBeforeDrop () {
			if (viewBeforeDrop == "")
				GotoPrevious ();
			else
				Goto (viewBeforeDrop);
		}

		/// <summary>
		/// Gets the view with the given id as the type T
		/// </summary>
		/// <param name="id">id of the view to get (leave blank to get the current view)</param>
		/// <returns>View as T</returns>
		public T GetView<T> (string id="") where T : View {
			if (id == "") id = CurrView;
			return (T)Views[id];
		}

		/// <summary>
		/// Gets the view with the given id
		/// </summary>
		/// <param name="id">id of the view to get (leave blank to get the current view)</param>
		/// <returns>View</returns>
		public View GetView (string id="") {
			if (id == "") id = CurrView;
			return Views[id];
		}

		/// <summary>
		/// Send all players to a new view
		/// </summary>
		/// <param name="id">id of the view to move to</param>
		public void AllGoto (string id) {
			Game.Dispatcher.ScheduleMessage ("GotoView", id);
		}

		/// <summary>
		/// Called when the player is disconnected. Passes the message on to the current view.
		/// </summary>
		public void OnDisconnect () {
			Views[CurrView].OnDisconnect ();
		}

		/// <summary>
		/// Called when a client (other than this one) is dropped or rejoins the game. Passes the message on to the current view.
		/// </summary>
		/// <param name="hasDroppedClients">True if clients are missing from the game, otherwise false</param>
		public void OnUpdateDroppedClients (bool hasDroppedClients) {
			if (hasDroppedClients) {
				if (CurrView != "dropped")
					viewBeforeDrop = CurrView;
				Views[CurrView].OnClientDropped ();
			} else {
				Views[CurrView].OnClientsReconnected ();
			}
		}

		/// <summary>
		/// Called when socket server for all clients is unresponsive.
		/// </summary>
		public void OnSocketDisconnected () {
			Views[CurrView].OnSocketDisconnected();
		}

		void OnGotoView (NetMessage msg) {
			Goto (msg.str1);
		}
	}
}