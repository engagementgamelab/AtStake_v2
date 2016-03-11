using UnityEngine;
using System.Collections;

public class DebugInfoContainer : MonoBehaviour {

	public MessageDispatcherDebug dispatcherDebug;

	public void Init (GameInstance game) {
		dispatcherDebug.Init (game.Dispatcher);
	}
}
