using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.IO;

/// <summary>
/// Handles local multiplayer over WIFI
/// </summary>
public class NetworkingManager : MonoBehaviour, IConnectionManager {

	// const bool enableNetworkClass = false;
	// const bool enableNetworkHudClass = false;

	public struct Settings {

		public readonly string GameName;
		public readonly int MaxConnections;
		public readonly bool SecureServer;
		public readonly float TimeoutDuration;
		public readonly int Attempts;

		public Settings (int maxConnections=5, bool secureServer=false, float timeoutDuration=10f, int attempts=3) {
			GameName = "@Stake";
			MaxConnections = maxConnections;
			SecureServer = secureServer;
			TimeoutDuration = timeoutDuration;
			Attempts = attempts;
		}
	}

	/*public ConnectionStatus Status {
		get { return status; }
	}*/

	/*NetworkManager netManager = null;
	NetworkManager NetManager {
		get {
			if (netManager == null) {
				netManager = GetComponent<NetworkManager> ();
				// netManager.playerPrefab = ObjectPool.Instantiate<GameInstance> ().gameObject;
				// netManager.enabled = enableNetworkClass;
			}
			return netManager;
		}
	}*/

	/*NetworkManagerHUD networkHUD = null;
	NetworkManagerHUD NetworkHUD {
		get {
			if (networkHUD == null) {
				networkHUD = GetComponent<NetworkManagerHUD> ();
				// networkHUD.enabled = enableNetworkClass && enableNetworkHudClass;
			}
			#if !SINGLE_SCREEN
			NetManager.enabled = false;
			#endif
			return networkHUD;
		}
	}*/

	/*NetworkMatch match = null;
	NetworkMatch Match {
		get {
			if (match == null) {
				match = gameObject.AddComponent<NetworkMatch> ();
			}
			return match;
		}
	}*/

	MyNetworkDiscovery discovery = null;
	MyNetworkDiscovery Discovery  {
		get {
			if (discovery == null) {
				discovery = gameObject.AddComponent<MyNetworkDiscovery> ();
				// discovery.useNetworkManager = true;
				discovery.useNetworkManager = false;
				/*if (!discovery.Initialize ()) {
					Debug.LogError ("Failed to initialize discovery service");
				}*/
			}
			discovery.broadcastData = gameInstanceName;
			return discovery;
		}
	}

	// NetworkClient client;

	public NetworkMasterServer server;
	public NetworkMasterClient client;

	string gameInstanceName;
	MultiplayerManager multiplayer;
	Settings settings;
	// NetworkConnectionTest test;
	// ConnectionStatus status = ConnectionStatus.Undetermined;
	// ConnectionStatus status = ConnectionStatus.Succeeded;

	void OnEnable () {

		settings = new Settings (4, false, 3f, 3);

		#if !SINGLE_SCREEN

		// Configure connection to multiplayer server
		/*MasterServer.ipAddress = DataManager.MultiplayerServerIp;
		MasterServer.port = DataManager.MultiplayerServerPort;
		Network.natFacilitatorIP = DataManager.MultiplayerServerIp;
		Network.natFacilitatorPort = DataManager.FacilitatorPort;

		// Test connection
		test = ObjectPool.Instantiate<NetworkConnectionTest> ();
		test.Init (settings);
		test.TestClientConnection (OnTestResult);*/

		#endif
	}

	public void Init (string gameInstanceName, MultiplayerManager multiplayer) {
		this.gameInstanceName = gameInstanceName;
		this.multiplayer = multiplayer;
	}

	public void Host () {

		server.InitializeServer ();
		client.InitializeClient (Network.player.ipAddress, () => {
			client.RegisterHost (settings.GameName, gameInstanceName, "", false, 4, 3148);//NetManager.networkPort);
			Discovery.StartBroadcasting ();
		});
	}

	public void Join (string hostName) {

	}

	public void RequestHostList (System.Action<List<string>> callback) {
		Discovery.StartListening ((Dictionary<string, string> hosts) => {
			callback (new List<string> (hosts.Keys));
		});
	}

	public void ConnectClient (string clientName) {
		
	}

	public void DisconnectClient () {

	}

	public void Disconnect (string hostName) {
		Discovery.Stop ();
		if (hostName == gameInstanceName) {
			client.UnregisterHost (() => {
				client.ResetClient ();
				Co.WaitForFixedUpdate (() => {
					server.ResetServer ();
				});
			});
		} else {
			client.ResetClient ();
		}
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
