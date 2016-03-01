using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalManager : MonoBehaviour, IConnectionManager {

	ConnectionStatus status;
	public ConnectionStatus Status {
		get { return status; }
	}

	public void Host (string gameInstanceName) {

	}

	public void Join (string hostName, string gameInstanceName) {
		List<GameInstance> gi = ObjectPool.GetActiveInstances<GameInstance> ();
		foreach (GameInstance g in gi) {
			if (g.Name == gameInstanceName) {
				Host = g;
				Host.Multiplayer.ConnectClient (gameInstanceName);
				return;
			}
		}
	}

	public List<string> UpdateHosts () {
		return null;
	}

	public void ConnectClient (string gameInstanceName) {

	}

	public void DisconnectClient (string gameInstanceName) {

	}

	public void Disconnect () {

	}
}
