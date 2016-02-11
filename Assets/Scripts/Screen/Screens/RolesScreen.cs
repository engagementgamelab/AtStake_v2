using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// Randomly assigns roles, with the last role being the Decider

public class RolesScreen : GameScreen {

	protected override void OnShow () {

		// Game.Dispatcher.AddListener ("AssignRole", AssignRole);
		
		if (IsHost) {

			string decider = GetDecider ();
			List<string[]> roles = GetRoles (decider);

			/*foreach (string title in Game.Decks.RoleTitles) {
				AddElement ("title_" + title, new TextElement (title));
			}*/

		}
	}

	string GetDecider () {
		List<string> potentialDeciders = Game.Manager.Players
			.Where (kv => !kv.Value.HasBeenDecider)
			.Select (kv => kv.Key).ToList ();
		return potentialDeciders[Random.Range (0, potentialDeciders.Count)];
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

	void AssignRole (NetworkMessage msg) {

	}
}
