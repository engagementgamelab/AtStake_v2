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
						{ "name", new Name () },
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
						{ "extra_time", new ExtraTime ("pitch") },
						{ "deliberate_instructions", new DeliberateInstructions () },
						{ "deliberate", new Deliberate () },
						{ "extra_time_deliberate", new ExtraTime ("deliberate") },
						{ "decide", new Decide () },
						{ "winner", new Winner () },
						{ "agenda_item", new AgendaItem () },
						{ "agenda_item_accept", new AgendaItemAccept () },
						{ "agenda_item_reject", new AgendaItemReject () },
						{ "scoreboard", new Scoreboard () },
						{ "final_scoreboard", new FinalScoreboard () }
					};
				}

				return views;
			}
		}

		public string CurrView { get; private set; }
		public string PrevView { get; private set; }

		public void Init () {
			foreach (var view in Views)
				view.Value.Init (this, view.Key);
			Goto ("start");
			Game.Dispatcher.AddListener ("GotoView", OnGotoView);
		}

		public void Goto (string id) {

			if (id == CurrView) return;

			// Unload the current view and update the PrevView reference
			if (!string.IsNullOrEmpty (CurrView)) {
				Views[CurrView].Hide ();
				PrevView = CurrView;
			}
			CurrView = id;

			// Render the new view
			try {
				Game.Templates.Load (CurrView, Views[CurrView]);
			} catch (KeyNotFoundException e) {
				throw new System.Exception ("No template with the id '" + id + "' exists.\n" + e);
			}
		}

		public void GotoPrevious () {
			Goto (PrevView);
		}

		void OnGotoView (MasterMsgTypes.GenericMessage msg) {
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