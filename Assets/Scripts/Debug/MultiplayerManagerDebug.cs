using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerManagerDebug : MonoBehaviour {

	public GenericList msgList;
	public GenericList hostList;
	public Button hostButton;
	public Button hostListButton;
	public Button disconnectButton;
	public Text broadcasterStatus;
	public Text listenerStatus;
	public Text hostStatus;
	public Text clientsStatus;
	MultiplayerManager2 multiplayer;

	public void Init (MultiplayerManager2 multiplayer) {
		this.multiplayer = multiplayer;
		#if SHOW_DEBUG_INFO
		multiplayer.onLogMessage += OnLogMessage;
		#endif
	}

	public void Host () {
		multiplayer.HostGame ();
	}

	public void RequestHostList () {
		multiplayer.RequestHostList ((List<string> hosts) => {

			List<GenericButton> buttons = hostList.transform.GetChildren<GenericButton> ();

			// Add new hosts
			foreach (string host in hosts) {
				if (buttons.Find (x => x.Id == host) == null) {
					hostList.AddButton (host, () => {
						multiplayer.JoinGame (host, (string response) => {
							Debug.Log (response);
						});
					});
				}
			}

			// Remove old hosts
			ObjectPool.DestroyChildrenWithCriteria<GenericButton> (hostList.transform, x => !hosts.Contains (x.Id));
		});
	}

	public void Disconnect () {
		multiplayer.Disconnect ();
	}

	void Update () {

		hostButton.gameObject.SetActive (
			!multiplayer.client.IsConnected 
			&& MasterServerDiscovery.Broadcaster == null 
			&& !MasterServerDiscovery.HasListener (multiplayer));

		hostListButton.gameObject.SetActive (
			!multiplayer.Connected 
			&& !MasterServerDiscovery.HasListener (multiplayer));

		disconnectButton.gameObject.SetActive (
			multiplayer.client.IsConnected
			|| MasterServerDiscovery.HasListener (multiplayer));

		broadcasterStatus.text = MasterServerDiscovery.Broadcaster == null ? "Not broadcasting" : "Broadcasting";
		listenerStatus.text = MasterServerDiscovery.HasListener (multiplayer) ? "Listening" : "Not listening";

		if (multiplayer.Connected) {
			hostStatus.text = "Host: " + multiplayer.Host;
			if (multiplayer.Hosting)
				hostStatus.text += " (me)";
			clientsStatus.text = "Players: ";
			foreach (string player in multiplayer.Game.Manager.PlayerNames) {
				clientsStatus.text += player + ", ";
			}
		} else {
			hostStatus.text = "not connected";
			clientsStatus.text = "";
		}
	}

	void OnLogMessage (string msg) {
		msgList.AddText (msg);
	}
}
