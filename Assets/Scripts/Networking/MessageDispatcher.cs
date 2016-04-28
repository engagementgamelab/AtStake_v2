using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The MessageDispatcher sends and receives messages between players.
/// </summary>
public class MessageDispatcher : GameInstanceBehaviour {

	// -- Public methods

	/// <summary>
	/// Sends a message to all clients
	/// </summary>
	/// <param name="id">The message id</param>
	public void ScheduleMessage (string id) {
		ScheduleMessage (NetMessage.Create (id));
	}

	/// <summary>
	/// Sends a message to all clients
	/// </summary>
	/// <param name="id">The message id</param>
	/// <param name="str1">String information to include in the message</param>
	public void ScheduleMessage (string id, string str1) {
		ScheduleMessage (NetMessage.Create (id, str1));
	}

	/// <summary>
	/// Sends a message to all clients
	/// </summary>
	/// <param name="id">The message id</param>
	/// <param name="val">Integer information to include in the message</param>
	public void ScheduleMessage (string id, int val) {
		ScheduleMessage (NetMessage.Create (id, "", "", val));
	}

	/// <summary>
	/// Sends a message to all clients
	/// </summary>
	/// <param name="id">The message id</param>
	/// <param name="str1">String information to include in the message</param>
	/// <param name="str2">String information to include in the message</param>
	public void ScheduleMessage (string id, string str1, string str2) {
		ScheduleMessage (NetMessage.Create (id, str1, str2));
	}

	/// <summary>
	/// Sends a message to all clients
	/// </summary>
	/// <param name="id">The message id</param>
	/// <param name="str1">String information to include in the message</param>
	/// <param name="str2">String information to include in the message</param>
	/// <param name="val">Integer information to include in the message</param>
	public void ScheduleMessage (string id, string str1, int val) {
		ScheduleMessage (NetMessage.Create (id, str1, "", val));
	}

	/// <summary>
	/// Sends a message to all clients
	/// </summary>
	/// <param name="id">The message id</param>
	/// <param name="val">Integer information to include in the message</param>
	/// <param name="bytes">byte[] information to include in the message</param>
	public void ScheduleMessage (string id, int val, byte[] bytes) {
		ScheduleMessage (NetMessage.Create (id, "", "", val, bytes));
	}

	/// <summary>
	/// Sends a message to all clients
	/// </summary>
	/// <param name="msg">The NetMessage to send</param>
	public void ScheduleMessage (NetMessage msg) {
		Game.Multiplayer.SendMessageToClients (msg);
		ReceiveMessageEvent (msg);
	}

	// -- Listeners

	public delegate void OnReceiveMessage (NetMessage msg);
	Dictionary<OnReceiveMessage, string> listeners = new Dictionary<OnReceiveMessage, string> ();

	#if SHOW_DEBUG_INFO
	public delegate void OnUpdateListeners (Dictionary<OnReceiveMessage, string> listeners);
	public OnUpdateListeners onUpdateListeners;
	#endif

	/// <summary>
	/// Listen to the message with the given id
	/// </summary>
	/// <param name="id">id of the message to listen for</param>
	/// <param name="action">The action to run when the message is received</param>
	public void AddListener (string id, OnReceiveMessage action) {

		if (!listeners.ContainsKey (action))
			listeners.Add (action, id);

		#if SHOW_DEBUG_INFO
		if (onUpdateListeners != null)
			onUpdateListeners (listeners);
		#endif
	}

	/// <summary>
	/// Remove a listener
	/// </summary>
	/// <param name="action">The action to remove</param>
	public void RemoveListener (OnReceiveMessage action) {

		listeners.Remove (action);

		#if SHOW_DEBUG_INFO
		if (onUpdateListeners != null)
			onUpdateListeners (listeners);
		#endif
	}

	/// <summary>
	/// Clears all listeners
	/// </summary>
	public void Reset () {

		listeners.Clear ();

		#if SHOW_DEBUG_INFO
		if (onUpdateListeners != null)
			onUpdateListeners (listeners);
		#endif
	}

	// -- Private methods

	/// <summary>
	/// MultiplayerManager calls this when a message is received. Better not to use this in any other context ;)
	/// </summary>
	public void ReceiveMessage (NetMessage msg) {
		#if SIMULATE_LATENCY
			StartCoroutine (LatentSendConfirmation (msg));
		#else
			ReceiveMessageEvent (msg);
		#endif
	}

	void ReceiveMessageEvent (NetMessage msg) {
		Dictionary<OnReceiveMessage, string> tempListeners = new Dictionary<OnReceiveMessage, string> (listeners);
		foreach (var listener in tempListeners) {
			if (listener.Value == msg.id)
				listener.Key (msg);
		}
	}

	#if SIMULATE_LATENCY
	IEnumerator LatentSendConfirmation (NetMessage msg) {
		yield return new WaitForSeconds (Random.value);
		ReceiveMessageEvent (msg);
	}
	#endif
}
