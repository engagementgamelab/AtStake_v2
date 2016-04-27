using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	//// <summary>
	/// Manages all the views in the game
	/// Sets which view is currently active
	/// </summary>
	public class ViewManager : GameInstanceBehaviour {

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
						{ "disconnected", new Disconnected () }
					};
				}

				return views;
			}
		}

		bool Loaded { get { return views != null; } }

		public string CurrView { get; private set; }
		public string PrevView { get; private set; }

		public void Init () {
			if (!Loaded) {
				foreach (var view in Views)
					view.Value.Init (this, view.Key);
				Goto ("start");
			}
			Game.Dispatcher.AddListener ("GotoView", OnGotoView);
		}

		public void Goto (string id) {

			Log	(id);
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

		public void GotoPrevious () {
			Goto (PrevView);
		}

		public T GetView<T> (string id="") where T : View {
			if (id == "") id = CurrView;
			return (T)Views[id];
		}

		public View GetView (string id="") {
			if (id == "") id = CurrView;
			return Views[id];
		}

		void OnGotoView (NetMessage msg) {
			Goto (msg.str1);
		}

		public void AllGoto (string id) {
			Game.Dispatcher.ScheduleMessage ("GotoView", id);
		}

		public void OnDisconnect () {
			Views[CurrView].OnDisconnect ();
		}
	}
}