using UnityEngine;
using System.Collections;

public class DebugInfoContainer : MonoBehaviour {

	public MessageDispatcherDebug dispatcherDebug;
	public ViewManagerDebug viewDebug;

	public void Init (GameInstance game) {
		dispatcherDebug.Init (game.Dispatcher);
		viewDebug.Init (game.Views);
	}
}
