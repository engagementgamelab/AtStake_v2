using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class DiscoveryService : MonoBehaviour {

	public static DiscoveryService Broadcaster { get; private set; }
	public static DiscoveryService Listener { get; private set; }

	string MasterAddress {
		get { return DataManager.MasterServerAddress; }
	}

	string hostName;
	string ipAddress;
	bool broadcasting = false;
	bool listening = false;
	HostsData received;

	Dictionary<MultiplayerManager, System.Action<Dictionary<string, string>>> onUpdateHostsDelegates = new Dictionary<MultiplayerManager, System.Action<Dictionary<string, string>>> ();

	int timeoutBuffer = 1;				// Maximum number of times a host can be missing from broadcastsReceived before they are removed from the host list
	float timeSinceLastReceived = 0f;	// Time (in seconds) that has passed since the last broadcast was received
	float broadcastTimeout = 1.5f;		// Time (in seconds) to wait for broadcasts. If the time passes beyond this, the host list is cleared

	class HostsData {

		public HostData[] hosts { get; set; }

		public Dictionary<string, string> Package () {
			Dictionary<string, string> data = new Dictionary<string, string> ();
			foreach (HostData h in hosts)
				data.Add (h.name, h.address);
			return data;
		}

		public void Clear () {
			hosts = new HostData[0];
		}
	}

	class HostData {
		public string name { get; set; }
		public string address { get; set; }
	}

	public static void StartBroadcasting (string hostName, string ipAddress) {

		if (Broadcaster != null)
			return;

		Broadcaster = ObjectPool.Instantiate<DiscoveryService> ();
		Broadcaster.hostName = hostName;
		Broadcaster.ipAddress = ipAddress;
		Broadcaster.broadcasting = true;

		Co.InvokeWhileTrue (0.5f, () => { return Broadcaster.broadcasting; }, () => {
			Co.WWW (Broadcaster.MasterAddress + "/addHost/" + Broadcaster.hostName + "/" + Broadcaster.ipAddress, (WWW www) => {
				// Debug.Log ("sent " + ipAddress);
			});
		}, () => {
			Co.WWW (Broadcaster.MasterAddress + "/removeHost/" + Broadcaster.hostName + "/" + Broadcaster.ipAddress, (WWW www) => {
				// Debug.Log ("removed " + ipAddress);
				ObjectPool.Destroy<DiscoveryService> (Broadcaster);
				Broadcaster = null;
			});
		});
	}

	public static void StopBroadcasting () {
		Broadcaster.broadcasting = false;
	}

	public static void StartListening (MultiplayerManager multiplayer, System.Action<Dictionary<string, string>> onUpdateHosts) {

		if (Listener != null) {
			#if SINGLE_SCREEN
			Listener.onUpdateHostsDelegates[multiplayer] = onUpdateHosts;
			#else
			Debug.LogWarning ("Discovery already listening");
			#endif
			return;
		}

		Listener = ObjectPool.Instantiate<DiscoveryService> ();
		Listener.onUpdateHostsDelegates[multiplayer] = onUpdateHosts;
		Listener.listening = true;

		Co.InvokeWhileTrue (0.5f, () => { return Listener.listening; }, () => {
			Co.WWW (Listener.MasterAddress + "/hosts", (WWW www) => {
				Listener.received = JsonReader.Deserialize<HostsData> (www.text);
				Listener.SendUpdateHostsMessage ();
			});
		}, () => {
			Debug.Log ("destroyed");
			ObjectPool.Destroy<DiscoveryService> (Listener);
			Listener = null;
		});
	}

	public static void StopListening (MultiplayerManager multiplayer) {

		if (Listener == null || !Listener.onUpdateHostsDelegates.ContainsKey (multiplayer))
			return;

		// Listener.onUpdateHostsDelegates[multiplayer] (new Dictionary<string, string> ());
		Listener.onUpdateHostsDelegates.Remove (multiplayer);
		if (Listener.onUpdateHostsDelegates.Count == 0)
			Listener.listening = false;
	}

	public static bool HasListener (MultiplayerManager multiplayer) {
		return Listener != null && Listener.onUpdateHostsDelegates.ContainsKey (multiplayer);
	}

	void SendUpdateHostsMessage () {
		Dictionary<string, string> hosts = received.Package ();
		foreach (var del in onUpdateHostsDelegates)
			del.Value (hosts);
	}
}
