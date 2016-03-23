using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class NetworkMasterClient2 : MonoBehaviour {

	public class Callbacks {
		
		Dictionary<string, System.Action> callbacks = new Dictionary<string, System.Action> () {
			{ "connect", null },
			{ "unregister_host", null },
			{ "disconnected", null }
		};

		// For these callbacks, listeners will be removed after the callback has been invoked
		List<string> singleInvoke = new List<string> () {
			"connect", "unregister_host"
		};

		public void AddListener (string key, System.Action action) {
			try {
				if (singleInvoke.Contains (key))
					callbacks[key] = null;
				callbacks[key] += action;
			} catch {
				throw new System.Exception ("No callback with the key '" + key + "' could be found");
			}
		}

		public void RemoveListener (string key, System.Action action) {
			try {
				callbacks[key] -= action;
			} catch {
				throw new System.Exception ("No callback with the key '" + key + "' could be found");
			}
		}

		// Call the callback with the given key. Optionally provide a string to pass along as well.
		public void Call (string key, string str="") {
			try {
				if (callbacks[key] != null) {
					callbacks[key] ();
					if (singleInvoke.Contains (key))
						callbacks[key] = null;
				}
			} catch {
				throw new System.Exception ("Could not trigger the callback because '" + key + "' has not been added to the dictionary of callbacks");
			}
		}
	}

	class Settings {

		public string MasterServerIpAddress = "255.255.255.255";
		public int MasterServerPort = 31485;
		public string GameTypeName = "@Stake";
		public int GamePort = 3148;
		public readonly bool LogMessagesInConsole = false;

		public string IpAddress {
			#if SINGLE_SCREEN
			get { return "127.0.0.1"; }
			#else
			get { return Network.player.ipAddress; }
			#endif
		}
	}

	public bool IsConnected {
		get {
			if (client == null) 
				return false;
			else 
				return client.isConnected;
		}
	}

	public delegate void OnClientMessage (string msg);

	public readonly Callbacks callbacks = new Callbacks ();
	Settings settings = new Settings ();
	NetworkClient client;

	public OnClientMessage onClientMessage;
	public System.Action<int, string> onRegisteredClient;

	public void StartAsHost (string hostName, System.Action onConnect) {
		Initialize (() => {
			RegisterHost (hostName);
			onConnect ();
		});
	}

	public void StartAsClient (string clientName, string hostIp, System.Action onConnect) {
		Initialize (() => {
			RegisterClient (clientName);
			onConnect ();
		}, hostIp);
	}

	void Initialize (System.Action onConnect, string ipAddress="") {

		if (client != null) {
			Log ("Already connected");
			return;
		}

		callbacks.AddListener ("connect", onConnect);
		client = new NetworkClient ();
		client.Connect (ipAddress == "" ? settings.IpAddress : ipAddress, settings.MasterServerPort);

		// system messages
		client.RegisterHandler (MsgType.Connect, OnConnect);
		client.RegisterHandler (MsgType.Disconnect, OnDisconnect);
		client.RegisterHandler (MsgType.Error, OnError);

		// application messages
		client.RegisterHandler (MasterMsgTypes.RegisteredHostId, OnRegisteredHost);
		client.RegisterHandler (MasterMsgTypes.UnregisteredHostId, OnUnregisteredHost);
		client.RegisterHandler(MasterMsgTypes.RegisteredClientId, OnRegisteredClient);
	}

	void Reset () {

		if (client == null)
			return;

		client.Disconnect();
		client = null;
	}

	void RegisterHost (string hostName) {

		if (!IsConnected) {
			Log ("Could not register host because client is not connected");
			return;
		}

		var msg = new MasterMsgTypes.RegisterHostMessage ();
		msg.gameTypeName = settings.GameTypeName;
		msg.gameName = hostName;
		msg.hostPort = settings.GamePort;

		client.Send (MasterMsgTypes.RegisterHostId, msg);
	}

	public void UnregisterHost (string hostName, System.Action onUnregister) {

		if (!IsConnected) {
			Log ("Could not unregister host because the client is not connected");
			return;
		}

		callbacks.AddListener ("unregister_host", onUnregister);

		var msg = new MasterMsgTypes.UnregisterHostMessage ();
		msg.gameTypeName = settings.GameTypeName;
		msg.gameName = hostName;
		client.Send (MasterMsgTypes.UnregisterHostId, msg);
	}

	void RegisterClient (string clientName) {

		var msg = new MasterMsgTypes.RegisterClientMessage ();
		msg.gameTypeName = settings.GameTypeName;
		msg.clientName = clientName;
		client.Send (MasterMsgTypes.RegisterClientId, msg);
	}

	// -- System Handlers

	void OnConnect (NetworkMessage netMsg) {
		callbacks.Call ("connect");
	}

	void OnDisconnect (NetworkMessage netMsg) {
		Log("Client Disconnected from Master");
		Reset ();
		callbacks.Call ("disconnected");
	}

	void OnError (NetworkMessage netMsg) {
		Log("ClientError from Master");
	}

	// -- Application Handlers

	void OnRegisteredHost (NetworkMessage netMsg) {
		Log ("Registered host");
	}

	void OnUnregisteredHost (NetworkMessage netMsg) {
		callbacks.Call ("unregister_host");
		callbacks.Call ("disconnected");
		Reset ();
		Log ("Unregistered host");
	}

	void OnRegisteredClient (NetworkMessage netMsg) {
		var msg = netMsg.ReadMessage<MasterMsgTypes.RegisteredClientMessage> ();
		if (onRegisteredClient != null)
			onRegisteredClient (msg.resultCode, msg.clientName);
	}

	// -- Debugging
	void Log (string msg) {
		if (settings.LogMessagesInConsole)
			Debug.Log (msg);
		if (onClientMessage != null)
			onClientMessage (msg);
	}
}
