using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerManagerDebug : MonoBehaviour {

	public Button disconnectButton;
	public Button dropButton;
	public Text broadcasterStatus;
	public Text listenerStatus;
	public Text hostStatus;
	public Text clientsStatus;
	MultiplayerManager multiplayer;

	public void Init (MultiplayerManager multiplayer) {
		this.multiplayer = multiplayer;
	}

	public void Disconnect () {
		multiplayer.Disconnect ();
	}

	public void Drop () {
		multiplayer.Drop ();
	}

	public void Reconnect () {
		
	}

	void Update () {

		if (multiplayer == null) return;

		disconnectButton.gameObject.SetActive (multiplayer.Connected);
		dropButton.gameObject.SetActive (multiplayer.Connected);
		
		broadcasterStatus.text = (multiplayer.Hosting && multiplayer.ConnectionStatus == ConnectionStatus.Success) 
			? "Broadcasting"
			: "Not broadcasting";

		listenerStatus.text = (!multiplayer.Connected && multiplayer.ConnectionStatus == ConnectionStatus.Success)
			? "Listening"
			: "Not listening";

		if (multiplayer.Connected) {
			hostStatus.text = "Host: " + multiplayer.Host;
			if (multiplayer.Hosting)
				hostStatus.text += " (me)";
			clientsStatus.text = "Players: ";
			foreach (var player in multiplayer.Game.Manager.Players) {
				clientsStatus.text += player.Key + ", ";
			}
		} else {
			hostStatus.text = "not connected";
			clientsStatus.text = "";
		}
	}
}
