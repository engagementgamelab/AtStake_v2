using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Simulates local multiplayer in a single screen
/// </summary>
public class LocalManager : MonoBehaviour, IConnectionManager {

	/*public ConnectionStatus Status {
		get { return ConnectionStatus.Succeeded; }
	}*/

	string gameInstanceName;

	public void Init (string gameInstanceName) {
		this.gameInstanceName = gameInstanceName;
	}

	public void Host () {}

	public void Join (string hostName) {
		FindGameWithName (hostName).Multiplayer.ConnectClient (gameInstanceName);
	}

	public void RequestHostList (System.Action<List<string>> callback) {
		callback (ObjectPool.GetActiveInstances<GameInstance> ()
			.FindAll (x => x.Name != gameInstanceName && x.Multiplayer.Hosting)
			.ConvertAll (x => x.Name));
	}

	public void ConnectClient (string clientName) {}

	public void DisconnectClient () {}

	public void Disconnect (string hostName) {
		if (hostName == gameInstanceName) {
			List<GameInstance> instances = ObjectPool.GetActiveInstances<GameInstance> ();
			foreach (GameInstance game in instances) {
				if (game.Multiplayer.Host == hostName)
					game.Multiplayer.Disconnect ();	
			}
		} else {
			FindHostAsMultiplayer ().DisconnectClient (gameInstanceName);
		}
	}

	public void SendMessageToHost (string id, string str1, string str2, int val) {
		FindHostAsMultiplayer ().ReceiveMessageFromClient (id, str1, str2, val);
	}

	public void ReceiveMessageFromClient (string id, string str1, string str2, int val) {
		FindHost ().Dispatcher.ReceiveMessageFromClient (id, str1, str2, val);
	}

	public void SendMessageToClients (string id, string str1, string str2, int val) {
		foreach (GameInstance instance in ObjectPool.GetActiveInstances<GameInstance> ().FindAll (x => !x.Multiplayer.Hosting)) {
			instance.Multiplayer.ReceiveMessageFromHost (id, str1, str2, val);
		}
	}

	public void ReceiveMessageFromHost (string id, string str1, string str2, int val) {
		FindGameWithName (gameInstanceName).Dispatcher.ReceiveMessageFromHost (id, str1, str2, val);
	}

	GameInstance FindGameWithName (string name) {
		return ObjectPool.GetActiveInstances<GameInstance> ().Find (x => x.Name == name);
	}

	GameInstance FindHost () {
		return ObjectPool.GetActiveInstances<GameInstance> ().Find (x => x.Multiplayer.Hosting);
	}

	MultiplayerManager FindHostAsMultiplayer () {
		return FindHost ().Multiplayer;
	}
}
