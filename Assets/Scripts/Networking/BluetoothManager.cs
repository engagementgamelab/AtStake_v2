using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles local multiplayer over Bluetooth
/// </summary>
public class BluetoothManager : MonoBehaviour, IConnectionManager {

	public ConnectionStatus Status {
		get { return status; }
	}

	string gameInstanceName;
	ConnectionStatus status;

	public void Init (string gameInstanceName) {
		this.gameInstanceName = gameInstanceName;
	}

	public void Host () {

	}

	public void Join (string hostName) {

	}

	public List<string> UpdateHosts () {
		return null;
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
