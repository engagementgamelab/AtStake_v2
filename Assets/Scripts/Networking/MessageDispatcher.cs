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

	public delegate void OnReceiveMessage (NetworkMessage msg);

	Queue<NetworkMessage> messages = new Queue<NetworkMessage> ();
	Confirmation confirmation = new Confirmation ("", new List<string> ());

	bool Hosting {
		get { return Game.Multiplayer.Hosting; }
	}

	List<string> Clients {
		get { return Game.Multiplayer.Clients; }
	}

	/**
	 *	Public methods
	 */

	public void ScheduleMessage (string id) {
		ScheduleMessage (new NetworkMessage (id));
	}

	public void ScheduleMessage (string id, string str1) {
		ScheduleMessage (new NetworkMessage (id, str1));
	}

	public void ScheduleMessage (string id, int val) {
		ScheduleMessage (new NetworkMessage (id, "", "", val));
	}

	public void ScheduleMessage (string id, string str1, string str2) {
		ScheduleMessage (new NetworkMessage (id, str1, str2));
	}

	public void ScheduleMessage (string id, string str1, int val) {
		ScheduleMessage (new NetworkMessage (id, str1, "", val));
	}

	public void ScheduleMessage (NetworkMessage msg) {
		if (Hosting) {
			QueueMessage (msg);
		} else {
			SendMessageToHost (msg);
		}
	}

	/**
	 *	Private methods
	 */

	// Client methods

	void SendMessageToHost (NetworkMessage msg) {
		Game.Multiplayer.SendMessageToHost (msg.id, msg.str1, msg.str2, msg.val);
	}

	// Host methods

	void QueueMessage (NetworkMessage msg) {
		if (messages.Count > 0 || !confirmation.IsConfirmed (Clients)) {
			messages.Enqueue (msg);
		} else {
			SendMessageToClients (msg);
		}
	}

	void SendQueuedMessage () {
		if (messages.Count > 0) {
			NetworkMessage msg = messages.Dequeue ();
			SendMessageToClients (msg);
		}
	}

	void SendMessageToClients (NetworkMessage msg) {

		// Create a new confirmation to fulfill
		confirmation = new Confirmation (msg.id, Clients);

		// Send message to all clients
		Game.Multiplayer.SendMessageToClients (msg.id, msg.str1, msg.str2, msg.val);

		// Fire off the message
		ReceiveMessageEvent (msg);
	}

	void ReceiveMessageEvent (NetworkMessage msg) {
		Dictionary<OnReceiveMessage, string> tempListeners = new Dictionary<OnReceiveMessage, string> (listeners);
		foreach (var listener in tempListeners) {
			if (listener.Value == msg.id) {
				listener.Key (msg);
			}
		}
	}

	// Listeners

	Dictionary<OnReceiveMessage, string> listeners = new Dictionary<OnReceiveMessage, string> ();

	public void AddListener (string id, OnReceiveMessage action) {
		if (!listeners.ContainsKey (action))
			listeners.Add (action, id);
	}

	public void RemoveListener (OnReceiveMessage action) {
		listeners.Remove (action);
	}

	public void RemoveAllListeners () {
		listeners.Clear ();
	}

	// RPCs

	public void ReceiveMessageFromClient (string id, string str1, string str2, int val) {
		if (id.Contains ("__confirm")) {
			ReceiveConfirmation (id.Replace("__confirm", ""), str1);
		} else {
			QueueMessage (new NetworkMessage (id, str1, str2, val));
		}
	}

	public void ReceiveMessageFromHost (string id, string str1, string str2, int val) {
		#if SIMULATE_LATENCY
			StartCoroutine (LatentSendConfirmation (id, str1, str2, val));
		#else
			SendMessageToHost (new NetworkMessage ("__confirm" + id, Game.Name, "", -1));
			ReceiveMessageEvent (new NetworkMessage (id, str1, str2, val));
		#endif
	}

	#if SIMULATE_LATENCY
	IEnumerator LatentSendConfirmation (string id, string str1, string str2, int val) {
		yield return new WaitForSeconds (Random.value);
		SendMessageToHost (new NetworkMessage ("__confirm" + id, Game.Name, "", -1));
		ReceiveMessageEvent (new NetworkMessage (id, str1, str2, val));
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

	#if UNITY_EDITOR

	bool showStatus = false;

	void OnGUI () {

		if (!Hosting) return;

		GUI.color = Color.black;
		showStatus = GUILayout.Toggle (showStatus, "Show message status");

		if (!showStatus) return;

		foreach (NetworkMessage msg in messages) {
			GUILayout.Label (msg.id);
		}

		GUILayout.Label (confirmation.messageId + " confirmed: " + confirmation.IsConfirmed (Clients));
	}
	#endif
}