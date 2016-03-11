using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class MessageDispatcherDebug : MonoBehaviour {

	public GenericList list;

	List<GenericList> targets = new List<GenericList> ();
	string[] excludes = new [] { "manager" }; // won't include targets that contain these strings

	public void Init (MessageDispatcher dispatcher) {
		#if SHOW_DEBUG_INFO
		dispatcher.onUpdateListeners += OnUpdateListeners;
		#endif
	}

	void OnUpdateListeners (Dictionary<MessageDispatcher.OnReceiveMessage, string> listeners) {

		foreach (GenericList target in targets)
			target.Clear<GenericText> ();

		ObjectPool.DestroyChildren<GenericList> (list.Transform);
		targets.Clear ();

		foreach (var listener in listeners) {

			string target = listener.Key.Target.ToString ();
			if (System.Array.Find (excludes, x => target.ToLower ().Contains (x)) != null)
				continue;

			GenericList targetList = targets.Find (x => x.Id == target);

			if (targetList == null) {

				// Create the new list and parent it
				targetList = ObjectPool.Instantiate<GenericList> ();
				targets.Add (targetList);
				targetList.Parent = transform;
				targetList.LocalScale = Vector3.one;
				targetList.LocalPosition = Vector3.zero;
				
				// Configure the new list
				targetList.Id = target;
				AddTextToList (targetList, target + ":", true);
			}

			AddTextToList (targetList, listener.Value);
		}
	}

	void AddTextToList (GenericList l, string s, bool bold=false) {
		GenericText text = ObjectPool.Instantiate<GenericText> ();
		text.Text.text = s;
		text.Style = bold ? FontStyle.Bold : FontStyle.Normal;
		l.Add<GenericText> (text);
	}
}
