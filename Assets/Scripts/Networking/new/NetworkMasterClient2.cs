using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class NetworkMasterClient2 : MonoBehaviour {

	class Callbacks {
		
		Dictionary<string, System.Action> callbacks = new Dictionary<string, System.Action> () {
			{ "connect", null },
			{ "unregister_host", null }
		};

		public void AddListener (string id, System.Action action) {
			try {
				callbacks[id] += action;
			} catch {
				throw new System.Exception ("No callback with the id '" + id + "' could be found");
			}
		}

		public void Call (string id) {
			try {
				if (callbacks[id] != null) {
					callbacks[id] ();
					callbacks[id] = null;
				}
			} catch {
				throw new System.Exception ("Could not trigger the callback because '" + id + "' has not been added to the dictionary of callbacks");
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

	Callbacks callbacks = new Callbacks ();
	Settings settings = new Settings ();
	NetworkClient client;

	public OnClientMessage onClientMessage;

	public void Initialize (System.Action onConnect=null) {

		if (client != null) {
			Log ("Already connected");
			return;
		}

		callbacks.AddListener ("connect", onConnect);
		client = new NetworkClient ();
		client.Connect (settings.IpAddress, settings.MasterServerPort);

		// system messages
		client.RegisterHandler (MsgType.Connect, OnConnect);
		client.RegisterHandler (MsgType.Disconnect, OnDisconnect);
		client.RegisterHandler (MsgType.Error, OnError);

		// application messages
		client.RegisterHandler (MasterMsgTypes.RegisteredHostId, OnRegisteredHost);
		client.RegisterHandler (MasterMsgTypes.UnregisteredHostId, OnUnregisteredHost);
	}

	public void Reset () {

		if (client == null)
			return;

		client.Disconnect();
		client = null;
	}

	public void RegisterHost (string gameName) {

		if (!IsConnected) {
			Log ("Could not register host because client is not connected");
			return;
		}

		var msg = new MasterMsgTypes.RegisterHostMessage ();
		msg.gameTypeName = settings.GameTypeName;
		msg.gameName = gameName;
		msg.hostPort = settings.GamePort;

		client.Send (MasterMsgTypes.RegisterHostId, msg);
	}

	public void UnregisterHost (string gameName, System.Action onUnregister) {

		if (!IsConnected) {
			Log ("Could not unregister host because the client is not connected");
			return;
		}

		callbacks.AddListener ("unregister_host", onUnregister);

		var msg = new MasterMsgTypes.UnregisterHostMessage ();
		msg.gameTypeName = settings.GameTypeName;
		msg.gameName = gameName;
		client.Send (MasterMsgTypes.UnregisterHostId, msg);
	}

	// -- System Handlers

	void OnConnect (NetworkMessage netMsg) {
		callbacks.Call ("connect");
	}

	void OnDisconnect (NetworkMessage netMsg) {
		Log("Client Disconnected from Master");
		Reset ();
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
		Reset ();
		Log ("Unregistered host");
	}

	// -- Debugging
	void Log (string msg) {
		if (settings.LogMessagesInConsole)
			Debug.Log (msg);
		if (onClientMessage != null)
			onClientMessage (msg);
	}
}
