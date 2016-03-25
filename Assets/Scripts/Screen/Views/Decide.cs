using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	public class Decide : View {

		string selectedPeer;

		protected override void OnInitDeciderElements () {

			// Confirmation button. Can only be pressed after a player has been selected.
			Elements.Add ("confirm", new ButtonElement ("Confirm", () => {
				Game.Dispatcher.ScheduleMessage ("ChooseWinner", selectedPeer);
			}) { Interactable = false });

			// A button list of players
			// Dictionary<string, ButtonElement> peers = Game.Manager.PeerNames.ToDictionary (x => x, x => new ButtonElement (x, () => {
			Dictionary<string, ButtonElement> peers = Game.Controller.PeerNames.ToDictionary (x => x, x => new ButtonElement (x, () => {
				selectedPeer = x;
				ButtonElement confirmButton = GetScreenElement<ButtonElement> ("confirm");
				confirmButton.Text = "Confirm " + selectedPeer;
				confirmButton.Interactable = true;
			}));

			Elements.Add ("peer_list", new ListElement<ButtonElement> (peers));
		}

		protected override void OnShow () { Game.Dispatcher.AddListener ("ChooseWinner", ChooseWinner); }
		protected override void OnHide () { Game.Dispatcher.RemoveListener (ChooseWinner); }

		void ChooseWinner (MasterMsgTypes.GenericMessage msg) {
			Game.Controller.SetWinner (msg.str1);
			GotoView ("winner");
		}
	}
}