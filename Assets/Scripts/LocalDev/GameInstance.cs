using UnityEngine;
using System.Collections;

public class GameInstance : MonoBehaviour {

	GameInstanceUI ui;
	public PlayerManager manager;
	public PlayerInstance player;
	bool focused = false;

	void Awake () {

		manager = ObjectPool.Instantiate<PlayerManager> ();
		manager.transform.SetParent (transform);
		player = ObjectPool.Instantiate<PlayerInstance> ();
		player.transform.SetParent (transform);

		Transform grid = GameObject.FindWithTag ("LocalDevGrid").transform;
		ui = ObjectPool.Instantiate<GameInstanceUI> ();
		ui.transform.SetParent (grid);
		ui.Init (this);
	}

	public void Focus () {
		ui.Focus ();
		focused = true;
	}

	public void Unfocus () {
		ui.Unfocus ();
		focused = false;
	}

	void Update () {
		/*if (focused) {
			
		}*/
	}
}
