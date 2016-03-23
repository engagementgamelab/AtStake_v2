using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MasterServerDiscovery : NetworkDiscovery {

	class BroadcastResult {

		public NetworkBroadcastResult Result { get; set; }
		public string GameName { get; set; }
		public int Timeout { get; set; }

		public BroadcastResult (string gameName) {
			Timeout = 0;
			GameName = gameName;
		}
	}

	public delegate void OnLogMessage (string msg);

	public static MasterServerDiscovery Broadcaster { get; private set; }
	public static MasterServerDiscovery Listener { get; private set; }
	public static OnLogMessage onLogMessage;

	Dictionary<string, BroadcastResult> received = new Dictionary<string, BroadcastResult> ();
	Dictionary<MultiplayerManager2, System.Action<Dictionary<string, string>>> onUpdateHostsDelegates = new Dictionary<MultiplayerManager2, System.Action<Dictionary<string, string>>> ();

	int timeoutBuffer = 1;				// Maximum number of times a host can be missing from broadcastsReceived before they are removed from the host list
	float timeSinceLastReceived = 0f;	// Time (in seconds) that has passed since the last broadcast was received
	float broadcastTimeout = 1.5f;		// Time (in seconds) to wait for broadcasts. If the time passes beyond this, the host list is cleared

	void OnEnable () {
		showGUI = false;
	}

	public static void StartBroadcasting (string gameName) {

		if (Broadcaster != null) {
			Debug.LogWarning ("Discovery already broadcasting");
			return;
		}

		Broadcaster = ObjectPool.Instantiate<MasterServerDiscovery> ();
		Broadcaster.useNetworkManager = false;

		// Hack to fix an infuriating unity bug where gameData was getting combined in NetworkDiscovery's OnReceivedBroadcast
		// The periods act as a buffer. They should be removed before the data is actually rendered to the screen.
		Broadcaster.broadcastData = gameName + "~~~~~~~~~~~~~~~~~~~~~~~~~~";

		Broadcaster.Initialize ();
		if (!Broadcaster.StartAsServer ()) {
			throw new System.Exception ("Failed to start network discovery");
		}
	}

	public static void StopBroadcasting () {

		if (Broadcaster == null)
			return;

		Broadcaster.StopBroadcast ();
		ObjectPool.Destroy<MasterServerDiscovery> (Broadcaster);
		Broadcaster = null;
	}

	public static void StartListening (MultiplayerManager2 multiplayer, System.Action<Dictionary<string, string>> onUpdateHosts) {

		if (Listener != null) {
			#if SINGLE_SCREEN
			Listener.onUpdateHostsDelegates[multiplayer] = onUpdateHosts;
			#else
			Debug.LogWarning ("Discovery already listening");
			#endif
			return;
		}

		Listener = ObjectPool.Instantiate<MasterServerDiscovery> ();
		Listener.useNetworkManager = false;

		Listener.Initialize ();
		if (Listener.StartAsClient ()) {
			Listener.onUpdateHostsDelegates[multiplayer] = onUpdateHosts;
			Listener.CheckForBroadcastTimeout ();
		} else {
			throw new System.Exception ("Failed to start discovery client");
		}
	}

	public static void StopListening (MultiplayerManager2 multiplayer) {

		if (Listener == null)
			return;

		Listener.onUpdateHostsDelegates[multiplayer] (new Dictionary<string, string> ());
		Listener.onUpdateHostsDelegates.Remove (multiplayer);
		if (Listener.onUpdateHostsDelegates.Count == 0) {
			Listener.received.Clear ();
			Listener.StopBroadcast ();
			ObjectPool.Destroy<MasterServerDiscovery> (Listener);
			Listener = null;
		}
	}

	public override void OnReceivedBroadcast (string fromAddress, string gameName) {

		// If this is a new broadcast, add it to the list of received broadcasts
		if (!received.ContainsKey (fromAddress))
			received[fromAddress] = new BroadcastResult (gameName.RemoveEmptyChars ());

		Dictionary<string, string> hosts = new Dictionary<string, string> ();

		// Populate the host list
		// Update the timers for all received broadcasts
		// Broadcasts that have not timed out are added to the host list
		foreach (var result in received) {

			string address = result.Key;
			BroadcastResult broadcastResult = result.Value;

			if (broadcastsReceived.ContainsKey (address)) {
				broadcastResult.Timeout = 0;
			} else {
				broadcastResult.Timeout ++;
				if (broadcastResult.Timeout >= received.Count + timeoutBuffer) {
					received.Remove (address);
					continue;
				}
			}

			hosts.Add (broadcastResult.GameName.Replace ("~", ""), address);
		}

		SendUpdateHostsMessage (hosts);
		timeSinceLastReceived = 0f;
	}

	public static bool HasListener (MultiplayerManager2 multiplayer) {
		return Listener != null && Listener.onUpdateHostsDelegates.ContainsKey (multiplayer);
	}

	void CheckForBroadcastTimeout () {
		Co.RunWhileTrue (() => { return Listener == this; }, () => {
			timeSinceLastReceived += Time.deltaTime;
			if (timeSinceLastReceived >= broadcastTimeout) {
				SendUpdateHostsMessage (new Dictionary<string, string> ());
				timeSinceLastReceived = 0f;
			}
		});
	}

	void SendUpdateHostsMessage (Dictionary<string, string> hosts) {
		foreach (var del in onUpdateHostsDelegates)
			del.Value (hosts);
	}

	static void Log (string msg) {
		if (onLogMessage != null)
			onLogMessage (msg);
	}
}
