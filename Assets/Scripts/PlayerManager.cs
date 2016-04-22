using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Models;

//// <summary>
/// Keeps track of all the players in the game
/// </summary>
public class PlayerManager : GameInstanceBehaviour {

	public delegate void OnAddPeer (string peer, string color);
	public delegate void OnRemovePeer (string peer);

	Dictionary<string, Player> players = new Dictionary<string, Player> ();
	public Dictionary<string, Player> Players {
		get { return players; }
	}

	public string Name { get; set; }
	public string TakenName { get; set; }
	public string AttemptedHost { get; set; }

	public OnAddPeer onAddPeer;
	public OnRemovePeer onRemovePeer;

	public void Init () {
		TakenName = "";
		Game.Dispatcher.AddListener ("UpdatePlayers", OnUpdatePlayers);
	}

	public void Reset () {
		players.Clear ();
	}

	public void AddHost (string color) {
		if (!players.ContainsKey (Name)) {
			players.Add (Name, new Player {
				Name = Name,
				Avatar = color
			});
			if (onAddPeer != null)
				onAddPeer (Name, color);
		}
	}

	public void OnUpdatePlayers (NetMessage msg) {

		Dictionary<string, string> playerColors = AvatarsManager.ToDict (msg.str1);

		// Add any players that haven't already been registered
		foreach (var pc in playerColors) {

			string name = pc.Key;
			string color = pc.Value;

			if (players.ContainsKey (name))
				continue;

			players.Add (name, new Player {
				Name = name,
				Avatar = color
			});

			if (onAddPeer != null)
				onAddPeer (name, color);
		}

		// Remove old players that weren't included in the message
		Dictionary<string, Player> temp = new Dictionary<string, Player> (players);

		foreach (var p in temp) {

			string name = p.Key;

			if (!playerColors.ContainsKey (name)) {

				players.Remove (name);

				if (onRemovePeer != null)
					onRemovePeer (name);
			}
		}
	}
}
