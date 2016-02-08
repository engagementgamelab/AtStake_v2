using UnityEngine;
using System.Collections;

public class GameInstance : MonoBehaviour {

	GameInstanceUI ui;
	public PlayerManager manager;
	public PlayerInstance player;
	public NetworkManager network;
	public MessageDispatcher dispatcher;
	bool focused = false;

	public string Name {
		get { return player.Data.Name; }
	}

	void Awake () {

		manager = ObjectPool.Instantiate<PlayerManager> ();
		manager.transform.SetParent (transform);

		player = ObjectPool.Instantiate<PlayerInstance> ();
		player.transform.SetParent (transform);

		network = ObjectPool.Instantiate<NetworkManager> ();
		network.transform.SetParent (transform);

		dispatcher = ObjectPool.Instantiate<MessageDispatcher> ();
		dispatcher.transform.SetParent (transform);
		dispatcher.Init (network);
		dispatcher.onReceiveMessage += OnReceiveMessage;

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

	public void AddLine (string line) {
		ui.AddTextLine (line);
	}

	void Update () {
		if (focused) {
			if (Input.GetKeyDown (KeyCode.H)) {
				HostGame ();
			}
			if (Input.GetKeyDown (KeyCode.F)) {
				network.UpdateHosts ();
			}
			if (Input.GetKeyDown (KeyCode.J)) {
				JoinGame ();
			}
			if (Input.GetKeyDown (KeyCode.A)) {
				dispatcher.ScheduleMessage ("balooga");
			}
			if (Input.GetKeyDown (KeyCode.S)) {
				dispatcher.ScheduleMessage ("frep");
			}
		}
	}

	public void HostGame () {
		network.HostGame ();
		manager.AddPlayer (player.Data);
		AddLine ("Hosting");
	}

	public void JoinGame () {
		network.UpdateHosts ();
		network.JoinGame (network.Hosts[0]);
		AddLine ("Joined " + network.Host.Name);
	}

	void OnReceiveMessage (NetworkMessage msg) {
		Debug.Log (Name + " ::: " + msg.id);
	}
}
