using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerManager2 : GameInstanceBehaviour {

	public delegate void OnLogMessage (string msg);

	public bool Hosting {
		get { return Host == Game.Name; }
	}

	public bool Connected {
		get { return false; }
	}

	public string Host { get; private set; }

	public List<string> Clients {
		get { return new List<string> (); }
	}

	public NetworkMasterServer2 server;
	public NetworkMasterClient2 client;
	public OnLogMessage onLogMessage;

	Dictionary<string, string> hosts;

	void OnEnable () {
		server.onServerMessage += SendLogMessage;
		client.onClientMessage += SendLogMessage;
		MasterServerDiscovery.onLogMessage += SendLogMessage;
	}

	public void HostGame () {
		Host = Game.Name;
		server.Initialize ();
		client.Initialize (() => {
			client.RegisterHost (Host);
			MasterServerDiscovery.StartBroadcasting (Host);
		});
	}

	public void RequestHostList (System.Action<List<string>> callback) {
		MasterServerDiscovery.StartListening (this, (Dictionary<string, string> hosts) => {
			this.hosts = hosts;
			callback (new List<string> (hosts.Keys));
		});
	}

	public void Disconnect () {
		if (Hosting) {
			MasterServerDiscovery.StopBroadcasting ();
			client.UnregisterHost (Host, () => {
				Co.WaitForFixedUpdate (() => {
					server.Reset ();
					Host = "";
				});
			});
		} else {
			MasterServerDiscovery.StopListening (this);
		}
	}

	// -- Messaging

	public void SendMessageToHost (MasterMsgTypes.GenericMessage msg) {

	}

	public void ReceiveMessageFromClient (MasterMsgTypes.GenericMessage msg) {

	}

	public void SendMessageToClients (MasterMsgTypes.GenericMessage msg) {

	}

	public void ReceiveMessageFromHost (MasterMsgTypes.GenericMessage msg) {

	}

	// -- Debugging

	void SendLogMessage (string msg) {
		if (onLogMessage != null)
			onLogMessage (msg);
	}
}
