#undef DEBUG
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkMasterClient : MonoBehaviour
{

	public delegate void OnReceiveMessageFromClient (MasterMsgTypes.GenericMessage msg);
	public delegate void OnReceiveMessageFromHost (MasterMsgTypes.GenericMessage msg);

	public bool dedicatedServer;
	public string MasterServerIpAddress;
	public int MasterServerPort;
	public int updateRate;
	public string gameTypeName;
	public string gameName;
	public int gamePort;

	[SerializeField]
	public int yoffset = 0;

	string HostGameType = "";
	string HostGameName = "";

	MasterMsgTypes.Room[] hosts = null;

	public NetworkClient client = null;

	static NetworkMasterClient singleton;

	// Callbacks
	System.Action onConnect;
	System.Action onDisconnect;
	System.Action onUnregisterHost;
	public System.Action<int, string> onRegisteredClient;
	public OnReceiveMessageFromClient onReceiveMessageFromClient;
	public OnReceiveMessageFromHost onReceiveMessageFromHost;

	void Awake()
	{
		if (singleton == null)
		{
			singleton = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	public void InitializeClient(string ipAddress="", System.Action onConnect=null)
	{
		if (client != null)
		{
			Debug.LogError("Already connected");
			return;
		}

		this.onConnect = onConnect;

		client = new NetworkClient();
		client.Connect(ipAddress == "" ? MasterServerIpAddress : ipAddress, MasterServerPort);

		// system msgs
		client.RegisterHandler(MsgType.Connect, OnClientConnect);
		client.RegisterHandler(MsgType.Disconnect, OnClientDisconnect);
		client.RegisterHandler(MsgType.Error, OnClientError);

		// application msgs
		client.RegisterHandler(MasterMsgTypes.RegisteredHostId, OnRegisteredHost);
		client.RegisterHandler(MasterMsgTypes.UnregisteredHostId, OnUnregisteredHost);
		client.RegisterHandler(MasterMsgTypes.ListOfHostsId, OnListOfHosts);
		client.RegisterHandler(MasterMsgTypes.RegisteredClientId, OnRegisteredClient);
		client.RegisterHandler(MasterMsgTypes.GenericHostFromClientId, OnHostFromClient);
		client.RegisterHandler(MasterMsgTypes.GenericClientsFromHostId, OnClientsFromHost);

	}

	public void ResetClient(System.Action onDisconnect=null)
	{
		if (client == null)
			return;

		client.Disconnect();
		// client.Shutdown (); // does this need to happen?
		client = null;
		hosts = null;
	}

	public bool isConnected
	{
		get
		{
			if (client == null) 
				return false;
			else 
				return client.isConnected;
		}
	}

	// --------------- System Handlers -----------------

	void OnClientConnect(NetworkMessage netMsg)
	{
		// Debug.Log("Client Connected to Master");
		if (onConnect != null)
			onConnect ();

	}

	void OnClientDisconnect(NetworkMessage netMsg)
	{
		Debug.Log("Client Disconnected from Master");
		ResetClient();
		OnFailedToConnectToMasterServer();
	}

	void OnClientError(NetworkMessage netMsg)
	{
		Debug.Log("ClientError from Master");
		OnFailedToConnectToMasterServer();
	}

	// --------------- Application Handlers -----------------

	void OnRegisteredHost(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisteredHostMessage>();
		OnServerEvent((MasterMsgTypes.NetworkMasterServerEvent)msg.resultCode);
	}

	void OnUnregisteredHost(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisteredHostMessage>();
		OnServerEvent((MasterMsgTypes.NetworkMasterServerEvent)msg.resultCode);
		if (onUnregisterHost != null)
			onUnregisterHost ();
	}

	void OnListOfHosts(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<MasterMsgTypes.ListOfHostsMessage>();
		hosts = msg.hosts;
		OnServerEvent(MasterMsgTypes.NetworkMasterServerEvent.HostListReceived);
	}

	void OnRegisteredClient(NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisteredClientMessage> ();
		if (onRegisteredClient != null)
			onRegisteredClient (msg.resultCode, msg.clientName);
	}

	void OnHostFromClient (NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<MasterMsgTypes.GenericMessage> ();
		if (onReceiveMessageFromClient != null)
			onReceiveMessageFromClient (msg);
	}

	void OnClientsFromHost (NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<MasterMsgTypes.GenericMessage> ();
		if (onReceiveMessageFromHost != null)
			onReceiveMessageFromHost (msg);
	}

	public void ClearHostList()
	{
		if (!isConnected)
		{
			Debug.LogError("ClearHostList not connected");
			return;
		}
		hosts = null;

	}

	public MasterMsgTypes.Room[] PollHostList()
	{
		if (!isConnected)
		{
			Debug.LogError("PollHostList not connected");
			return null;
		}
		return hosts;
	}

	public void RegisterHost(string gameTypeName, string gameName, string comment, bool passwordProtected, int playerLimit, int port)
	{
		if (!isConnected)
		{
			Debug.LogError("RegisterHost not connected");
			return;
		}

		var msg = new MasterMsgTypes.RegisterHostMessage();
		msg.gameTypeName = gameTypeName;
		msg.gameName = gameName;
		msg.comment = comment;
		msg.passwordProtected = passwordProtected;
		msg.playerLimit = playerLimit;
		msg.hostPort = port;
		client.Send(MasterMsgTypes.RegisterHostId, msg);

		HostGameType = gameTypeName;
		HostGameName = gameName;
	}

	public void RequestHostList(string gameTypeName)
	{
		if (!isConnected)
		{
			Debug.LogError("RequestHostList not connected");
			return;
		}

		var msg = new MasterMsgTypes.RequestHostListMessage();
		msg.gameTypeName = gameTypeName;
		client.Send(MasterMsgTypes.RequestListOfHostsId, msg);
	}

	public void UnregisterHost(System.Action onUnregisterHost=null)
	{
		if (!isConnected)
		{
			Debug.LogError("UnregisterHost not connected");
			return;
		}

		this.onUnregisterHost = onUnregisterHost;

		var msg = new MasterMsgTypes.UnregisterHostMessage();
		msg.gameTypeName = HostGameType;
		msg.gameName = HostGameName;
		client.Send(MasterMsgTypes.UnregisterHostId, msg);
		HostGameType = "";
		HostGameName = "";

		Debug.Log("send UnregisterHost");
	}

	public void RegisterClient(string gameTypeName, string clientName, string gameName)
	{
		var msg = new MasterMsgTypes.RegisterClientMessage();
		msg.gameTypeName = gameTypeName;
		msg.clientName = clientName;
		msg.gameName = gameName;
		client.Send(MasterMsgTypes.RegisterClientId, msg);
	}

	public void SendMessageToHost (MasterMsgTypes.GenericMessage msg) {
		client.Send (MasterMsgTypes.GenericClientToHostId, msg);
	}

	public void SendMessageToClients (MasterMsgTypes.GenericMessage msg) {
		client.Send (MasterMsgTypes.GenericHostToClientsId, msg);
	}

	public virtual void OnFailedToConnectToMasterServer()
	{
		Debug.Log("OnFailedToConnectToMasterServer");
	}

	public virtual void OnServerEvent(MasterMsgTypes.NetworkMasterServerEvent evt)
	{
		Debug.Log("OnServerEvent " + evt);

		if (evt == MasterMsgTypes.NetworkMasterServerEvent.HostListReceived)
		{
			Debug.Log ("found " + hosts.Length + " hosts");
			foreach (var h in hosts)
			{
				Debug.Log("Host:" + h.name + "addr:" + h.hostIp + ":" + h.hostPort);
			}
		}

		if (evt == MasterMsgTypes.NetworkMasterServerEvent.RegistrationSucceeded)
		{
			if (NetworkManager.singleton != null)
			{
				NetworkManager.singleton.StartHost();
			}
		}

		if (evt == MasterMsgTypes.NetworkMasterServerEvent.UnregistrationSucceeded)
		{
			if (NetworkManager.singleton != null)
			{
				NetworkManager.singleton.StopHost();
			}
		}
	}

	#if DEBUG
	void OnGUI()
	{
		if (client != null && client.isConnected)
		{
			if (GUI.Button(new Rect(100, 20+yoffset, 200, 20), "MasterClient Disconnect"))
			{
				ResetClient();
				if (NetworkManager.singleton != null)
				{
					NetworkManager.singleton.StopServer();
					NetworkManager.singleton.StopClient();
				}
				HostGameType = "";
				HostGameName = "";
			}
		}
		else
		{
			if (GUI.Button(new Rect(100, 20+yoffset, 200, 20), "MasterClient Connect"))
			{
				InitializeClient();
			}
			return;
		}


		if (HostGameType == "")
		{
			GUI.Label(new Rect(100, 50 + yoffset, 80, 20), "GameType:");
			gameTypeName = GUI.TextField(new Rect(180, 50 + yoffset, 200, 20), gameTypeName);

			GUI.Label(new Rect(100, 70 + yoffset, 80, 20), "GameName:");
			gameName = GUI.TextField(new Rect(180, 70 + yoffset, 200, 20), gameName);

			if (GUI.Button(new Rect(100, 90 + yoffset, 200, 20), "RegisterHost"))
			{
				int port = gamePort;
				if (NetworkManager.singleton != null)
				{
					port = NetworkManager.singleton.networkPort;
				}
				RegisterHost(gameTypeName, gameName, "none", false, 8, port);
			}

			if (GUI.Button(new Rect(100, 120 + yoffset, 200, 20), "List Hosts"))
			{
				RequestHostList(gameTypeName);
			}
		}
		else
		{
			if (GUI.Button(new Rect(100, 120 + yoffset, 120, 20), "UnregisterHost"))
			{
				UnregisterHost();
			}
		}

		if (hosts != null)
		{
			int y = 140;
			foreach (var h in hosts)
			{
				if (GUI.Button(new Rect(120, y + yoffset, 240, 20), "Host:" + h.name + "addr:" + h.hostIp + ":" + h.hostPort))
				{
					if (NetworkManager.singleton != null)
					{
						NetworkManager.singleton.networkAddress = h.hostIp;
						NetworkManager.singleton.networkPort = h.hostPort;
						NetworkManager.singleton.StartClient();
					}
				}
				y += 22;
			}
		}
	}
	#endif
}
