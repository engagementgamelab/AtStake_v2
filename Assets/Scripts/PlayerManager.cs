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
			if (!players.ContainsKey (myName))
				players.Add (myName, new Player { Name = myName });
		}
	}

	public OnAddPeer onAddPeer;
	public OnRemovePeer onRemovePeer;

	string[] avatarColors = new string[] {
		"red", "green", "orange", "pink", "yellow"
	};

	public void Init () {
		Game.Dispatcher.AddListener ("UpdatePlayers", OnUpdatePlayers);
	}

	public void Reset () {
		players.Clear ();
	}

	public void OnUpdatePlayers (MasterMsgTypes.GenericMessage msg) {

		List<string> playersStr = new List<string> (msg.str1.Split ('|'));

		foreach (string newPeer in playersStr) {
			if (!players.ContainsKey (newPeer)) {
				players.Add (newPeer, new Player { Name = newPeer });
				if (onAddPeer != null)
					onAddPeer (newPeer);
			}
		}

		Dictionary<string, Player> tempPeers = new Dictionary<string, Player> (players);

		foreach (var peer in tempPeers) {
			string peerName = peer.Key;
			if (!playersStr.Contains (peerName)) {
				players.Remove (peerName);
				if (onRemovePeer != null)
					onRemovePeer (peerName);
			}
		}
	}
}
