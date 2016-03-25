using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Models;

//// <summary>
/// Keeps track of all the players in the game
/// Updates player models to reflect who's the Decider and who's the winner of the round
/// </summary>
public class PlayerManager : GameInstanceBehaviour {

	public delegate void OnAddPeer (string peer);
	public delegate void OnRemovePeer (string peer);

	// All players (including this player)
	public Dictionary<string, Player> Players {
		get {
			Dictionary<string, Player> players = new Dictionary<string, Player> (peers);
			players.Add (Player.Name, Player);
			return players;
		}
	}

	// The players' names (including this one)
	public List<string> PlayerNames {
		get { return new List<string> (Players.Keys); }
	}

	// Every player except this one
	Dictionary<string, Player> peers = new Dictionary<string, Player> ();
	public Dictionary<string, Player> Peers {
		get { return peers; }
	}

	// The peers' names
	public List<string> PeerNames {
		get { return new List<string> (Peers.Keys); }
	}

	public string Decider { get; private set; }

	public Player DeciderPlayer {
		get { return Players[Decider]; }
	}

	public string Winner { get; set; }

	// This player
	Player player;
	public Player Player {
		get {
			if (player == null) {
				player = new Models.Player ();
			}
			return player;
		}
	}

	public OnAddPeer onAddPeer;
	public OnRemovePeer onRemovePeer;

	public void Init () {
		Game.Dispatcher.AddListener ("UpdatePlayers", OnUpdatePlayers);
		Game.Dispatcher.AddListener ("AssignRoles", AssignRoles);
		peers.Clear ();
		Player.HasBeenDecider = false;
	}

	public void OnUpdatePlayers (MasterMsgTypes.GenericMessage msg) {

		List<string> players = new List<string> (msg.str1.Split ('|'));
		players.Remove (Player.Name);

		foreach (string newPeer in players) {
			if (!peers.ContainsKey (newPeer)) {
				peers.Add (newPeer, new Player { Name = newPeer });
				if (onAddPeer != null)
					onAddPeer (newPeer);
			}
		}

		Dictionary<string, Player> tempPeers = new Dictionary<string, Player> (peers);

		foreach (var peer in tempPeers) {
			string peerName = peer.Key;
			if (!players.Contains (peerName)) {
				peers.Remove (peerName);
				if (onRemovePeer != null)
					onRemovePeer (peerName);
			}
		}
	}

	public List<string[]> ReadRoles (string roles) {

		List<string[]> convertedRoles = new List<string[]> ();
		List<string> playerRoles = new List<string> (roles.Split ('|'));

		foreach (string role in playerRoles) {
			string[] playerRole = role.Split(',');
			convertedRoles.Add (new string[] { playerRole[0], playerRole[1] } );
		}

		return convertedRoles;
	}

	void AssignRoles (MasterMsgTypes.GenericMessage msg) {
		foreach (string[] playerRole in ReadRoles (msg.str1)) {
			string player = playerRole[0];
			string role = playerRole[1];
			Players[player].Role = Game.Decks.GetRole (role);
			if (role == "Decider") {
				Decider = player;
				Players[Decider].HasBeenDecider = true;
			}	
		}
	}

	public void AddPeer (string name) {
		peers.Add (name, new Player { Name = name });
	}

	public void RemovePeer (string name) {
		peers.Remove (name);
	}
}
