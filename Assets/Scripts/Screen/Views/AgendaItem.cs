using UnityEngine;
using System.Collections;
using Models;

namespace Views {

	public class AgendaItem : View {

		PlayerAgendaItem Item {
			get { return Game.Controller.CurrentAgendaItem; }
		}

		protected override void OnInitDeciderElements () {
			Elements.Add ("accept", new ButtonElement (Model.Buttons["accept"], () => { AllGotoView ("agenda_item_accept"); }, "click2"));
			Elements.Add ("reject", new ButtonElement (Model.Buttons["reject"], () => { AllGotoView ("agenda_item_reject"); }));
		}

		protected override void OnInitPlayerElements () {
			// if (Item.PlayerName == Name)
				// Elements.Add ("your_item", new TextElement (Model.Text["your_item"]));
		}

		protected override void OnInitElements () {
			Elements.Add ("item", new TextElement (Item.Description));
		}
	}
}