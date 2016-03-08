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
public class NetworkingManager : MonoBehaviour {

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
				discovery.useNetworkManager = false;
			}
			// Hack to fix an infuriating unity bug where gameData was getting combined in NetworkDiscovery's OnReceivedBroadcast
			// The periods act as a buffer. They get removed when the data is actually rendered to the screen.
			discovery.broadcastData = gameInstanceName + "....................";
			return discovery;
		}
	}

	string IpAddress {
		#if SINGLE_SCREEN
		get { return "127.0.0.1"; }
		#else
		get { return Network.player.ipAddress; }
		#endif
	}

	int Port {
		get { return 3148; }
	}

	public NetworkMasterServer server;
	public NetworkMasterClient client;

	string gameInstanceName;
	MultiplayerManager multiplayer;
	Settings settings;
	Dictionary<string, string> hosts;
	// NetworkConnectionTest test;
	// ConnectionStatus status = ConnectionStatus.Undetermined;
	// ConnectionStatus status = ConnectionStatus.Succeeded;

	void OnEnable () {
		settings = new Settings (4, false, 3f, 3);
		client.onRegisteredClient += (int resultCode, string clientName) => {
			multiplayer.OnRegisteredClient (resultCode, clientName);
		};
		client.onReceiveMessageFromHost += (MasterMsgTypes.GenericMessage msg) => {
			multiplayer.ReceiveMessageFromHost (msg);
		};
		client.onReceiveMessageFromClient += (MasterMsgTypes.GenericMessage msg) => {
			multiplayer.ReceiveMessageFromClient (msg);
		};
	}

	public void Init (string gameInstanceName, MultiplayerManager multiplayer) {
		this.gameInstanceName = gameInstanceName;
		this.multiplayer = multiplayer;
	}

	public void Host () {

		server.InitializeServer ();
		client.InitializeClient (IpAddress, () => {
			string gameName = gameInstanceName;
			client.RegisterHost (settings.GameName, gameName, "", false, 4, Port);
			Discovery.StartBroadcasting ();
		});
	}

	public void Join (string hostName) {
		client.InitializeClient (hosts[hostName], () => {
			client.RegisterClient (settings.GameName, gameInstanceName, hostName);
			Discovery.Stop ();
		});
	}

	public void RequestHostList (System.Action<List<string>> callback) {
		Discovery.StartListening ((Dictionary<string, string> hosts) => {
			this.hosts = hosts;
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

	public void SendMessageToHost (MasterMsgTypes.GenericMessage msg) {
		client.SendMessageToHost (msg);
	}

	public void SendMessageToClients (MasterMsgTypes.GenericMessage msg) {
		client.SendMessageToClients (msg);
	}
}
