using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Models;

//// <summary>
/// Keeps track of all the players in the game
/// </summary>
public class PlayerManager : GameInstanceBehaviour {

	public delegate void OnAddPeer (string peer);
	public delegate void OnRemovePeer (string peer);

	Dictionary<string, Player> players = new Dictionary<string, Player> ();
	public List<Player> Players {
		get { return new List<Player> (players.Values); }
	}

	public List<string> PlayerNames {
		get { return new List<string> (players.Keys); }
	}

	string myName;
	public string Name {
		get { return myName; }
		set {
			myName = value;
			// if (!players.ContainsKey (myName))
				// players.Add (myName, new Player { Name = myName });
		}
	}

	public OnAddPeer onAddPeer;
	public OnRemovePeer onRemovePeer;

	public void Init () {
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
				onAddPeer (Name);
		}
	}

	public void OnUpdatePlayers (MasterMsgTypes.GenericMessage msg) {

		Dictionary<string, string> playerColors = AvatarsManager.ToDict (msg.str1);

		// Add any players that haven't already been registered
		foreach (var pc in playerColors) {

			string name = pc.Key;
			if (players.ContainsKey (name))
				continue;

			players.Add (name, new Player {
				Name = name,
				Avatar = pc.Value
			});

			if (onAddPeer != null)
				onAddPeer (name);
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
