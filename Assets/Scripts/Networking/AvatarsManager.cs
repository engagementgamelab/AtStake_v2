using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AvatarsManager {

	Dictionary<string, string> players = new Dictionary<string, string> ();

	string[] colors = new string[] { "red", "green", "orange", "pink", "yellow" };
	Stack<string> colorOrder;

	public string this[string player] {
		get { return players[player]; }
	}

	public AvatarsManager () {
		ShuffleColors ();
	}

	public void AddPlayer (string name) {
		players[name] = colorOrder.Pop ();
	}

	public void RemovePlayer (string name) {
		colorOrder.Push (players[name]);
		players.Remove (name);
	}

	public string GetPlayers () {
		string arr = "";
		foreach (var player in players)
			arr += player.Key + "," + player.Value + "|";
		arr = arr.Substring (0, arr.Length-1);
		return arr;
	}

	public static Dictionary<string, string> ToDict (string msg) {
		string[] keyVals = msg.Split ('|');
		Dictionary<string, string> playerColors = new Dictionary<string, string> ();
		foreach (string keyVal in keyVals) {
			string[] kv = keyVal.Split (',');
			playerColors.Add (kv[0], kv[1]);
		}
		return playerColors;
	}

	void ShuffleColors () {
		colorOrder = new Stack<string> (colors.ToList<string> ().ToShuffled ());
	}
}
