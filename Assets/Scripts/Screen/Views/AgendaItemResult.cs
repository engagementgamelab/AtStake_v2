using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Models;

namespace Views {

	public class AgendaItemResult : View {

		PlayerAgendaItem item;
		protected PlayerAgendaItem Item {
			get {
				if (item == null)
					item = Game.Controller.CurrentAgendaItem;
				return item;
			}
		}

		protected bool MyItem { get { return Item.PlayerName == Name; } }

		protected int[] RewardValues {
			get { return DataManager.GetSettings ().Rewards; }
		}

		protected Dictionary<string, string> RewardTextVariables {
			get { 
				Dictionary<string, string> v = new Dictionary<string, string> (TextVariables);
				v.Add ("reward", RewardValues[Item.Reward].ToString ());
				v.Add ("rewarded_player", Item.PlayerName);
				return v;
			}
		}

		protected override void OnInitDeciderElements () {
			Elements.Add ("next", new NextButtonElement ("", Advance));
		}

		protected override void OnHide () {
			item = null;
		}

		void Advance () {
			if (Game.Controller.NextAgendaItem ()) {
				AllGotoView ("agenda_item");
			} else {
				if (Game.Controller.NextRound ()) {
					AllGotoView ("scoreboard");
				} else {
					AllGotoView ("final_scoreboard");
				}
			}
		}
	}
}