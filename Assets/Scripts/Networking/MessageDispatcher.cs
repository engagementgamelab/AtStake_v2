using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageDispatcher : GameInstanceComponent {

	/**
	 *	The MessageDispatcher sends and receives messages between players.
	 *	In order to ensure synchronicity, messages go through the following steps:
	 *		1. A device sends a message to the host (or host sends the message to itself)
	 *		2. The host receives the message
	 *		3. The host sends the message to all clients
	 *		4. Clients, upon receiving the message, send a confirmation message back to the host
	 *		5. The host receives confirmations from clients
	 *		6. Once the host sees that all clients have confirmed receipt, it fires off 
	 *				the next message if one exists in the queue
	 */

	public delegate void OnReceiveMessage (NetworkMessage msg);

	public OnReceiveMessage onReceiveMessage;

	NetworkManager network;
	Queue<NetworkMessage> messages = new Queue<NetworkMessage> ();
	Confirmation confirmation = null;

	bool Hosting {
		get { return network.Hosting; }
	}

	/**
	 *	Public methods
	 */

	public void Init (NetworkManager network) {
		this.network = network;
	}

	public void ScheduleMessage (string id) {
		ScheduleMessage (new NetworkMessage (id));
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
		network.Host.dispatcher.ReceiveMessageFromClient (msg.id, msg.str1, msg.str2, msg.val);
	}

	// Host methods

	void QueueMessage (NetworkMessage msg) {
		if (messages.Count > 0 || confirmation != null) {
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
		confirmation = new Confirmation (msg.id, new List<string> (network.Clients.Keys));

		// Send message to all clients
		foreach (var client in network.Clients) {
			client.Value.dispatcher.ReceiveMessageFromHost (msg.id, msg.str1, msg.str2, msg.val);
		}

		// Fire off the message
		ReceiveMessageEvent (msg);
	}

	void ReceiveMessageEvent (NetworkMessage msg) {
		if (onReceiveMessage != null)
			onReceiveMessage (msg);
	}

	// RPCs

	public void ReceiveMessageFromClient (string id, string str1, string str2, int val) {
		QueueMessage (new NetworkMessage (id, str1, str2, val));
	}

	public void ReceiveMessageFromHost (string id, string str1, string str2, int val) {
		// StartCoroutine (SimulateSendConfirmation (id, str1, str2, val));
		network.Host.dispatcher.ReceiveConfirmation (id, Game.Name);
		ReceiveMessageEvent (new NetworkMessage (id, str1, str2, val));
	}

	/*IEnumerator SimulateSendConfirmation (string id, string str1, string str2, int val) {
		yield return new WaitForSeconds (Random.value);
		network.Host.dispatcher.ReceiveConfirmation (id, Game.Name);
		ReceiveMessageEvent (new NetworkMessage (id, str1, str2, val));
	}*/

	public void ReceiveConfirmation (string id, string client) {
		if (confirmation.Add (id, client)) {
			confirmation = null;
			SendQueuedMessage ();
		}
	}

	// Class that handles message confirmations
	// When the host sends a new message, it listens for clients to confirm that they've received it
	// It's only after all clients have confirmed that the host will send out subsequent messages
	public class Confirmation {

		readonly string messageId;
		readonly Dictionary<string, bool> confirms;

		public Confirmation (string messageId, List<string> clients) {
			this.messageId = messageId;
			confirms = new Dictionary<string, bool> ();
			foreach (string client in clients) {
				confirms.Add (client, false);
			}
		}

		// returns true if all clients have confirmed
		public bool Add (string id, string client) {
			if (id == messageId) {
				confirms[client] = true;
				foreach (var c in confirms) {
					if (!c.Value)
						return false;	
				}
				return true;
			}
			Debug.LogWarning ("Host received confirmation for the message '" + id + "' but the host is looking for confirmations for the message '" + messageId + "'");
			return false;
		}
	}
}