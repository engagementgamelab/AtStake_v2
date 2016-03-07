#undef DEBUG
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MyNetworkDiscovery : NetworkDiscovery {

	class BroadcastResult {

		public NetworkBroadcastResult Result { get; set; }
		public string GameName { get; set; }
		public int Timeout { get; set; }

		public BroadcastResult (string gameName) {
			Timeout = 0;
			GameName = gameName;
		}
	}

	Dictionary<string, string> gameNames = new Dictionary<string, string> ();
	Dictionary<string, BroadcastResult> received = new Dictionary<string, BroadcastResult> ();

	System.Action<Dictionary<string, string>> onUpdateHosts;
	int timeoutBuffer = 0; // Number of updates that can happen on a "missing" address before it is removed

	public void StartBroadcasting () {

		if (running) {
			#if DEBUG
			Debug.LogWarning ("Discovery already running");
			#endif
			return;
		}

		Initialize ();
		if (StartAsServer ()) {
			#if DEBUG
			Debug.Log ("Started discovery server");
			#endif
		} else {
			throw new System.Exception ("Failed to start discovery server");
		}
	}

	public void StartListening (System.Action<Dictionary<string, string>> onUpdateHosts) {

		if (running) {
			#if DEBUG
			Debug.LogWarning ("Discovery already running");
			#endif
			return;
		}

		Initialize ();
		if (StartAsClient ()) {
			#if DEBUG
			Debug.Log ("Started discovery client");
			#endif
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

		if (!received.ContainsKey (fromAddress)) {
			received[fromAddress] = new BroadcastResult (gameName);
		} else {
			received[fromAddress].Timeout = 0;
		}

		Dictionary<string, string> hosts = new Dictionary<string, string> ();

		#if DEBUG
		Debug.Log ("============ HOSTS ==============");
		#endif
		foreach (var broadcast in broadcastsReceived) {

			string address = broadcast.Value.serverAddress;

			if (!received.ContainsKey (address))
				continue;

			string name = received[address].GameName;
			if (received[address].Timeout >= received.Count + timeoutBuffer) {
				received.Remove (address);
			} else {
				received[address].Timeout ++;
			}
			#if DEBUG
			Debug.Log (name + ": " + address);
			#endif
			hosts.Add (name, address);
		}

		if (onUpdateHosts != null)
			onUpdateHosts (hosts);
	}
}
