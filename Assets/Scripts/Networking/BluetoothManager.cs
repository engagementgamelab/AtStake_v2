using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles local multiplayer over Bluetooth
/// </summary>
public class BluetoothManager : MonoBehaviour, IConnectionManager {

	/*public ConnectionStatus Status {
		get { return status; }
	}*/

	// string gameInstanceName;
	// ConnectionStatus status;

	public void Init (string gameInstanceName, MultiplayerManager multiplayer) {
		// this.gameInstanceName = gameInstanceName;
		// this.multiplayer = multiplayer;
	}

	public void Host () {

	}

	public void Join (string hostName) {

	}

	public void RequestHostList (System.Action<List<string>> callback) {
		
	}

	public void ConnectClient (string clientName) {
		
	}

	public void DisconnectClient () {

	}

	public void Disconnect (string hostName) {

	}

	public void SendMessageToHost (string id, string str1, string str2, int val) {
		
	}

	public void ReceiveMessageFromClient (string id, string str1, string str2, int val) {
		
	}

	public void SendMessageToClients (string id, string str1, string str2, int val) {

	}

	public void ReceiveMessageFromHost (string id, string str1, string str2, int val) {
		
	}
}
