using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

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

	NetworkManager netManager = null;
	NetworkManager NetManager {
		get {
			if (netManager == null) {
				netManager = GetComponent<NetworkManager> ();
				// netManager.playerPrefab = ObjectPool.Instantiate<GameInstance> ().gameObject;
				// netManager.enabled = enableNetworkClass;
			}
			return netManager;
		}
	}

	NetworkManagerHUD networkHUD = null;
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
	}

	/*NetworkMatch match = null;
	NetworkMatch Match {
		get {
			if (match == null) {
				match = gameObject.AddComponent<NetworkMatch> ();
			}
			return match;
		}
	}*/

	NetworkDiscovery discovery = null;
	NetworkDiscovery Discovery  {
		get {
			if (discovery == null) {
				discovery = gameObject.AddComponent<NetworkDiscovery> ();
				discovery.useNetworkManager = true;
				discovery.broadcastData = gameInstanceName;
				if (!discovery.Initialize ()) {
					Debug.LogError ("Failed to initialize discovery service");
				}
			}
			return discovery;
		}
	}

	// NetworkClient client;

	string gameInstanceName;
	// Settings settings;
	// NetworkConnectionTest test;
	// ConnectionStatus status = ConnectionStatus.Undetermined;
	// ConnectionStatus status = ConnectionStatus.Succeeded;

	void OnEnable () {

		// settings = new Settings (4, false, 3f, 3);

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

	/*void OnTestResult (ConnectionStatus result) {
		status = ConnectionStatus.Succeeded;
	}*/

	public void Init (string gameInstanceName) {
		this.gameInstanceName = gameInstanceName;
	}

	public void Host () {

		/*NetManager.networkAddress = "127.0.0.1";
		NetManager.StartHost();
		NetManager.StartMatchMaker();
		NetManager.matchName = gameInstanceName;
		NetManager.matchMaker.CreateMatch (NetManager.matchName, NetManager.matchSize, true, "", NetManager.OnMatchCreate);*/
		// NetManager.SetMatchHost("localhost", 8888, false);

		/*if (Discovery.StartAsServer ()) {
			Debug.Log ("Started local server");
		} else {
			Debug.LogError ("Failed to start server");
		}*/

		/*CreateMatchRequest create = new CreateMatchRequest ();
		create.name = gameInstanceName;
		create.size = 4;
		create.advertise = true;
		create.password = "";
		Match.CreateMatch (create, OnMatchCreate);*/

		/*if (settings.SecureServer)
			Network.InitializeSecurity ();
		Network.InitializeServer (settings.MaxConnections, 25001, !Network.HavePublicAddress ());
		MasterServer.RegisterHost (settings.GameName, gameInstanceName);*/
	}

	public void Join (string hostName) {

	}

	public void RequestHostList (System.Action<List<string>> callback) {
		// StartCoroutine (CoFindHosts (callback));
		/*if (Discovery.StartAsClient ()) {
			Debug.Log ("Successfully started client");
		} else {
			Debug.LogError ("Failed to start client");
		}*/
	}

	/*IEnumerator CoFindHosts (System.Action<List<string>> callback) {
		int attempts = settings.Attempts;
		while (attempts > 0) {
			attempts --;			
			yield return StartCoroutine (CoRequestHostList (attempts == 0, callback));
		}
	}

	IEnumerator CoRequestHostList (bool finalAttempt, System.Action<List<string>> callback) {
		float timeout = settings.TimeoutDuration;
		HostData[] hosts = new HostData[0];
		MasterServer.RequestHostList (settings.GameName);

		while (hosts.Length == 0 && timeout > 0) {
			hosts = MasterServer.PollHostList ();
			timeout -= Time.deltaTime;
			yield return null;
		}

		if (timeout <= 0f) {
			if (finalAttempt) {
				Debug.Log ("timeout");
			}
		} else {
			callback (new List<string> (new List<HostData> (hosts).ConvertAll (x => x.gameName)));
		}

		MasterServer.ClearHostList ();
	}*/

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

	// Events

	/*public void OnMatchCreate (CreateMatchResponse response) {
		if (response.success) {
			Debug.Log ("match created");
			Utility.SetAccessTokenForNetwork (response.networkId, new NetworkAccessToken(response.accessTokenString));
            NetworkServer.Listen (new MatchInfo (response), 9000);
		} else {
			Debug.LogError ("Create match failed");
		}
	}*/
}
