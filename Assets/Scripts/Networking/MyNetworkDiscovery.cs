#define DEBUG
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MyNetworkDiscovery : NetworkDiscovery {

	// public delegate void OnUpdateHosts (Dictionary<string, string> games);

	Dictionary<string, string> gameNames = new Dictionary<string, string> ();

	// public OnUpdateHosts onUpdateHosts;
	System.Action<Dictionary<string, string>> onUpdateHosts;

	public void StartBroadcasting () {

		if (running) {
			Debug.LogWarning ("Discovery already running");
			return;
		}

		Initialize ();
		if (StartAsServer ()) {
			Debug.Log ("Started discovery server");
		} else {
			throw new System.Exception ("Failed to start discovery server");
		}
	}

	public void StartListening (System.Action<Dictionary<string, string>> onUpdateHosts) {

		if (running) {
			Debug.LogWarning ("Discovery already running");
			return;
		}

		Initialize ();
		if (StartAsClient ()) {
			Debug.Log ("Started discovery client");
			this.onUpdateHosts += onUpdateHosts;
		} else {
			throw new System.Exception ("Failed to start discovery client");
		}
	}

	public void Stop () {
		onUpdateHosts = null;
		StopBroadcast ();
	}

	public override void OnReceivedBroadcast (string fromAddress, string gameName) {

		gameNames[fromAddress] = gameName;

		#if DEBUG
		Debug.Log ("============ HOSTS ==============");
		#endif
		Dictionary<string, string> hosts = new Dictionary<string, string> ();
		foreach (var broadcast in broadcastsReceived) {
			string address = broadcast.Value.serverAddress;
			string name = gameNames[address];
			hosts.Add (name, address);
			#if DEBUG
			Debug.Log (name + ": " + address);
			#endif
		}
		if (onUpdateHosts != null)
			onUpdateHosts (hosts);
	}
}
