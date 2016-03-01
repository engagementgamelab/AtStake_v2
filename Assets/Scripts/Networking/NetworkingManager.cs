using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkingManager : MonoBehaviour, IConnectionManager {

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

	public ConnectionStatus Status {
		get { return status; }
	}

	MultiplayerManager multiplayer;
	Settings settings;
	NetworkConnectionTest test;
	ConnectionStatus status = ConnectionStatus.Undetermined;

	void OnEnable () {

		settings = new Settings (4, false, 3f, 3);

		#if !SINGLE_SCREEN

		// Configure connection to multiplayer server
		MasterServer.ipAddress = DataManager.MultiplayerServerIp;
		MasterServer.port = DataManager.MultiplayerServerPort;
		Network.natFacilitatorIP = DataManager.MultiplayerServerIp;
		Network.natFacilitatorPort = DataManager.FacilitatorPort;

		// Test connection
		test = ObjectPool.Instantiate<NetworkConnectionTest> ();
		test.Init (settings);
		test.TestClientConnection (OnTestResult);

		#endif
	}

	void OnTestResult (ConnectionStatus result) {
		status = ConnectionStatus.Succeeded;
	}

	public void Init (MultiplayerManager multiplayer) {
		this.multiplayer = multiplayer;
	}

	public void Host (string gameInstanceName) {
		if (settings.SecureServer)
			Network.InitializeSecurity ();
		Network.InitializeServer (settings.MaxConnections, 25001, !Network.HavePublicAddress ());
		MasterServer.RegisterHost (settings.GameName, gameInstanceName);
	}

	public void Join (string hostName, string gameInstanceName) {

	}

	public List<string> UpdateHosts () {
		return null;
	}

	public void ConnectClient (string gameInstanceName) {
		
	}

	public void DisconnectClient (string gameInstanceName) {

	}

	public void Disconnect () {

	}
}
