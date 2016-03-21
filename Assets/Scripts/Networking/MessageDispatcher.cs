﻿#undef DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageDispatcher : GameInstanceBehaviour {

	/**
	 *	The MessageDispatcher sends and receives messages between players.
	 *	In order to ensure synchronicity, messages go through the following steps:
	 *		1. A device sends a message to the host (or, if hosting, skip to step 3)
	 *		2. The host receives the message
	 *		3. The host sends the message to all clients
	 *		4. Clients, upon receiving the message, send a confirmation message back to the host
	 *		5. The host receives confirmations from clients
	 *		6. Once the host sees that all clients have confirmed receipt, it fires off 
	 *				the next message if one exists in the queue
	 */

	public delegate void OnReceiveMessage (MasterMsgTypes.GenericMessage msg);

	Queue<MasterMsgTypes.GenericMessage> messages = new Queue<MasterMsgTypes.GenericMessage> ();
	Confirmation confirmation = new Confirmation ("", new List<string> ());

	bool Hosting {
		// get { return Game.Multiplayer.Hosting; }
		get { return Game.Multiplayer2.Hosting; }
	}

	List<string> Clients {
		// get { return Game.Multiplayer.Clients; }
		get { return Game.Multiplayer2.Clients; }
	}

	/**
	 *	Public methods
	 */

	public void ScheduleMessage (string id) {
		ScheduleMessage (MasterMsgTypes.GenericMessage.Create (id));
	}

	public void ScheduleMessage (string id, string str1) {
		ScheduleMessage (MasterMsgTypes.GenericMessage.Create (id, str1));
	}

	public void ScheduleMessage (string id, int val) {
		ScheduleMessage (MasterMsgTypes.GenericMessage.Create (id, "", "", val));
	}

	public void ScheduleMessage (string id, string str1, string str2) {
		ScheduleMessage (MasterMsgTypes.GenericMessage.Create (id, str1, str2));
	}

	public void ScheduleMessage (string id, string str1, int val) {
		ScheduleMessage (MasterMsgTypes.GenericMessage.Create (id, str1, "", val));
	}

	public void ScheduleMessage (MasterMsgTypes.GenericMessage msg) {
		if (Hosting) {
			QueueMessage (msg);
		} else {
			SendMessageToHost (msg);
		}
	}

	// Listeners

	Dictionary<OnReceiveMessage, string> listeners = new Dictionary<OnReceiveMessage, string> ();

	#if SHOW_DEBUG_INFO
	public delegate void OnUpdateListeners (Dictionary<OnReceiveMessage, string> listeners);
	public OnUpdateListeners onUpdateListeners;
	#endif

	public void AddListener (string id, OnReceiveMessage action) {
		if (!listeners.ContainsKey (action))
			listeners.Add (action, id);
		#if SHOW_DEBUG_INFO
		if (onUpdateListeners != null)
			onUpdateListeners (listeners);
		#endif
	}

	public void RemoveListener (OnReceiveMessage action) {
		listeners.Remove (action);
		#if SHOW_DEBUG_INFO
		if (onUpdateListeners != null)
			onUpdateListeners (listeners);
		#endif
	}

	public void RemoveAllListeners () {
		listeners.Clear ();
		#if SHOW_DEBUG_INFO
		if (onUpdateListeners != null)
			onUpdateListeners (listeners);
		#endif
	}

	/**
	 *	Private methods
	 */

	// Client methods

	void SendMessageToHost (MasterMsgTypes.GenericMessage msg) {
		// Game.Multiplayer.SendMessageToHost (msg);
		Game.Multiplayer2.SendMessageToHost (msg);
	}

	// Host methods

	void QueueMessage (MasterMsgTypes.GenericMessage msg) {
		if (messages.Count > 0 || !confirmation.IsConfirmed (Clients)) {
			messages.Enqueue (msg);
		} else {
			SendMessageToClients (msg);
		}
	}

	void SendQueuedMessage () {
		if (messages.Count > 0) {
			MasterMsgTypes.GenericMessage msg = messages.Dequeue ();
			SendMessageToClients (msg);
		}
	}

	void SendMessageToClients (MasterMsgTypes.GenericMessage msg) {

		// Create a new confirmation to fulfill
		confirmation = new Confirmation (msg.id, Clients);

		// Send message to all clients
		// Game.Multiplayer.SendMessageToClients (msg);
		// Game.Multiplayer2.SendMessageToClients (msg);

		// Fire off the message
		ReceiveMessageEvent (msg);
	}

	void ReceiveMessageEvent (MasterMsgTypes.GenericMessage msg) {
		Dictionary<OnReceiveMessage, string> tempListeners = new Dictionary<OnReceiveMessage, string> (listeners);
		foreach (var listener in tempListeners) {
			if (listener.Value == msg.id) {
				listener.Key (msg);
			}
		}
	}

	public void ReceiveMessageFromClient (MasterMsgTypes.GenericMessage msg) {
		if (msg.id.Contains ("__confirm")) {
			ReceiveConfirmation (msg.id.Replace("__confirm", ""), msg.str1);
		} else {
			QueueMessage (msg);
		}
	}

	public void ReceiveMessageFromHost (MasterMsgTypes.GenericMessage msg) {
		#if SIMULATE_LATENCY
			StartCoroutine (LatentSendConfirmation (msg));
		#else
			SendMessageToHost (MasterMsgTypes.GenericMessage.Create ("__confirm" + msg.id, Game.Name, "", -1));
			ReceiveMessageEvent (msg);
		#endif
	}

	#if SIMULATE_LATENCY
	IEnumerator LatentSendConfirmation (MasterMsgTypes.GenericMessage msg) {
		yield return new WaitForSeconds (Random.value);
		SendMessageToHost (MasterMsgTypes.GenericMessage.Create ("__confirm" + msg.id, Game.Name, "", -1));
		ReceiveMessageEvent (msg);
	}
	#endif

	void ReceiveConfirmation (string id, string client) {
		confirmation.Add (id, client);
		if (confirmation.IsConfirmed (Clients)) {
			SendQueuedMessage ();
		}
	}

	// Class that handles message confirmations
	// When the host sends a new message, it listens for clients to confirm that they've received it
	// It's only after all clients have confirmed that the host will send out subsequent messages
	public class Confirmation {

		public readonly string messageId;
		readonly Dictionary<string, bool> confirms;

		public Confirmation (string messageId, List<string> clients) {
			this.messageId = messageId;
			confirms = new Dictionary<string, bool> ();
			foreach (string client in clients) {
				confirms.Add (client, false);
			}
		}

		public bool IsConfirmed (List<string> clients) {
			foreach (string client in clients) {
				if (confirms.ContainsKey (client)) {
					if (!confirms[client])
						return false;
				}
			}
			return true;
		}

		public void Add (string id, string client) {
			if (id == messageId)
				confirms[client] = true;
		}
	}

	#if UNITY_EDITOR && DEBUG

	bool showStatus = false;

	void OnGUI () {

		if (!Hosting) return;

		GUI.color = Color.black;
		showStatus = GUILayout.Toggle (showStatus, "Show message status");

		if (!showStatus) return;

		foreach (MasterMsgTypes.GenericMessage msg in messages) {
			GUILayout.Label (msg.id);
		}

		GUILayout.Label (confirmation.messageId + " confirmed: " + confirmation.IsConfirmed (Clients));
	}
	#endif
}
