﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// Randomly assigns roles, with the last role being the Decider
	// Everyone sees everyone else's role

	public class Roles : View {

		ListElement<TextElement> roleList;

		protected override void OnInitElements () {
			roleList = new ListElement<TextElement> ();
			Elements.Add ("role_list", roleList);
			Elements.Add ("next", new NextButtonElement ("pot") { Active = false });
		}

		protected override void OnShow () {

			Game.Manager.Player.Role = null;
			Game.Dispatcher.AddListener ("AssignRole", AssignRole);
			
			if (IsHost) {

				string decider = GetDecider ();
				List<string[]> roles = GetRoles (decider);

				foreach (string[] role in roles) {
					Game.Dispatcher.ScheduleMessage ("AssignRole", role[0], role[1]);
				}
				Game.Dispatcher.ScheduleMessage ("AssignRole", decider, "Decider");
			}
		}

		string GetDecider () {
			List<string> potentialDeciders = Game.Manager.Players
				.Where (kv => !kv.Value.HasBeenDecider)
				.Select (kv => kv.Key).ToList ();
			try {
				return potentialDeciders[Random.Range (0, potentialDeciders.Count)];
			} catch {
				throw new System.Exception ("A decider could not be selected because all players have been deciders");
			}
		}

		List<string[]> GetRoles (string decider) {

			List<string> titles = Game.Decks.RoleTitles;
			List<string> players = Game.Manager.PlayerNames;
			players.Remove (decider);

			int[] randomIndices = new int[players.Count];
			int titlesCount = titles.Count;
			List<int> roleIndices = new List<int> ();
			
			// Generate a list of numbers iterating by 1
			for (int i = 0; i < titlesCount; i ++)
				roleIndices.Add (i);
			
			// Randomly assign the numbers to an array
			for (int i = 0; i < randomIndices.Length; i ++) {
				int r = Random.Range (0, roleIndices.Count);
				randomIndices[i] = roleIndices[r];
				roleIndices.Remove (roleIndices[r]);
			}

			List<string[]> roles = new List<string[]> ();
			for (int i = 0; i < players.Count; i ++) {
				roles.Add (new string[] { players[i], titles[i] });
			}
			return roles;
		}

		void AssignRole (MasterMsgTypes.GenericMessage msg) {
			roleList.Add (msg.str1, new TextElement (msg.str1 + ": " + msg.str2));
			if (IsDecider) {
				Elements["next"].Active = true;
			}
		}
	}
}