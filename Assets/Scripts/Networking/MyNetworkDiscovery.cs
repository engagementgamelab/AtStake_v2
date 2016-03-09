#undef DEBUG
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MyNetworkDiscovery : NetworkDiscovery {

	#if SINGLE_SCREEN
	public static MyNetworkDiscovery broadcasting { get; private set; }
	public static MyNetworkDiscovery listening { get; private set; }
	#endif

	class BroadcastResult {

		public NetworkBroadcastResult Result { get; set; }
		public string GameName { get; set; }
		public int Timeout { get; set; }

		public BroadcastResult (string gameName) {
			Timeout = 0;
			GameName = gameName;
		}
	}

	Dictionary<string, BroadcastResult> received = new Dictionary<string, BroadcastResult> ();

	System.Action<Dictionary<string, string>> onUpdateHosts;
	int timeoutBuffer = 0; // Number of updates that can happen on a "missing" address before it is removed

	void OnEnable () { showGUI = false; }

	public void StartBroadcasting () {

		if (running
			#if SINGLE_SCREEN
			|| broadcasting != null
			#endif
			) {
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
			#if SINGLE_SCREEN
			broadcasting = this;
			#endif
		} else {
			throw new System.Exception ("Failed to start discovery server");
		}
	}

	public void StartListening (System.Action<Dictionary<string, string>> onUpdateHosts) {

		if (running
			#if SINGLE_SCREEN
			|| listening != null
			#endif
			) {
			#if DEBUG
			Debug.LogWarning ("Discovery already running");
			#endif
			#if SINGLE_SCREEN
			if (!running)
				listening.onUpdateHosts += onUpdateHosts;
			#endif
			return;
		}

		Initialize ();
		if (StartAsClient ()) {
			#if DEBUG
			Debug.Log ("Started discovery client");
			#endif
			#if SINGLE_SCREEN
			listening = this;
			#endif
			this.onUpdateHosts += onUpdateHosts;
		} else {
			throw new System.Exception ("Failed to start discovery client");
		}
	}

	public void Stop () {
		onUpdateHosts = null;
		#if SINGLE_SCREEN
		if (broadcasting == this)
			broadcasting = null;
		else if (listening == this)
			listening = null;
		if (running)
		#endif
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
			hosts.Add (name.Replace (".", ""), address);
		}

		if (onUpdateHosts != null)
			onUpdateHosts (hosts);
	}
}
