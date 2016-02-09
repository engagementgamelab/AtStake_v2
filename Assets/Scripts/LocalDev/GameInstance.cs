using UnityEngine;
using System.Collections;

public class GameInstance : MonoBehaviour {

	public GameInstanceUI Ui { get; private set; }
	public PlayerManager Manager { get; private set; }
	public PlayerInstance Player { get; private set; }
	public NetworkManager Network { get; private set; }
	public MessageDispatcher Dispatcher { get; private set; }
	public GameScreenManager Screens { get; private set; }
	bool focused = false;

	public string Name {
		get { return Player.Data.Name; }
	}

	void Awake () {

		Manager = ObjectPool.Instantiate<PlayerManager> ();
		Manager.transform.SetParent (transform);

		Player = ObjectPool.Instantiate<PlayerInstance> ();
		Player.transform.SetParent (transform);

		Network = ObjectPool.Instantiate<NetworkManager> ();
		Network.transform.SetParent (transform);

		Dispatcher = ObjectPool.Instantiate<MessageDispatcher> ();
		Dispatcher.transform.SetParent (transform);
		Dispatcher.onReceiveMessage += OnReceiveMessage;

		Transform grid = GameObject.FindWithTag ("LocalDevGrid").transform;
		Ui = ObjectPool.Instantiate<GameInstanceUI> ();
		Ui.transform.SetParent (grid);
		Ui.Init (this);
		
		Screens = ObjectPool.Instantiate<GameScreenManager> ();
		Screens.transform.SetParent (transform);
		Screens.Init (Ui.Transform);
	}

	public void Focus () {
		Ui.Focus ();
		focused = true;
	}

	public void Unfocus () {
		Ui.Unfocus ();
		focused = false;
	}

	public void AddLine (string line) {
		Ui.AddTextLine (line);
	}

	/*void Update () {
		if (focused) {
			if (Input.GetKeyDown (KeyCode.H)) {
				HostGame ();
			}
			if (Input.GetKeyDown (KeyCode.F)) {
				Network.UpdateHosts ();
			}
			if (Input.GetKeyDown (KeyCode.J)) {
				JoinGame ();
			}
			if (Input.GetKeyDown (KeyCode.A)) {
				Dispatcher.ScheduleMessage ("balooga");
			}
			if (Input.GetKeyDown (KeyCode.S)) {
				Dispatcher.ScheduleMessage ("frep");
			}
		}
	}*/

	public void HostGame () {
		Network.HostGame ();
		Manager.AddPlayer (Player.Data);
		// AddLine ("Hosting");
	}

	public void JoinGame () {
		Network.UpdateHosts ();
		Network.JoinGame (Network.Hosts[0]);
		// AddLine ("Joined " + network.Host.Name);
	}

	void OnReceiveMessage (NetworkMessage msg) {
		Debug.Log (Name + " ::: " + msg.id);
	}
}
