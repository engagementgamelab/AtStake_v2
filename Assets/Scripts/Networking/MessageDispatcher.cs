using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The MessageDispatcher sends and receives messages between players.
/// </summary>
public class MessageDispatcher : GameInstanceBehaviour {

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

	public void ScheduleMessage (string id, int val, byte[] bytes) {
		ScheduleMessage (MasterMsgTypes.GenericMessage.Create (id, "", "", val, bytes));
	}

	public void ScheduleMessage (MasterMsgTypes.GenericMessage msg) {
		Game.Multiplayer.SendMessageToClients (msg);
		ReceiveMessageEvent (msg);
	}

	// Listeners

	public delegate void OnReceiveMessage (MasterMsgTypes.GenericMessage msg);
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

	public void Reset () {
		listeners.Clear ();
		#if SHOW_DEBUG_INFO
		if (onUpdateListeners != null)
			onUpdateListeners (listeners);
		#endif
	}

	/**
	 *	Private methods
	 */

	void ReceiveMessageEvent (MasterMsgTypes.GenericMessage msg) {
		Dictionary<OnReceiveMessage, string> tempListeners = new Dictionary<OnReceiveMessage, string> (listeners);
		foreach (var listener in tempListeners) {
			if (listener.Value == msg.id)
				listener.Key (msg);
		}
	}

	public void ReceiveMessage (MasterMsgTypes.GenericMessage msg) {
		#if SIMULATE_LATENCY
			StartCoroutine (LatentSendConfirmation (msg));
		#else
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
}
