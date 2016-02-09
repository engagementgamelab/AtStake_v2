using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInstance : MonoBehaviour {

	public GameInstanceUI Ui { get; private set; }
	public PlayerManager Manager { get; private set; }
	public NetworkManager Network { get; private set; }
	public MessageDispatcher Dispatcher { get; private set; }
	public GameScreenManager Screens { get; private set; }
	bool focused = false;

	public string Name {
		get { return Manager.Player.Name; }
	}

	void Awake () {

		Manager = ObjectPool.Instantiate<PlayerManager> ();
		Manager.transform.SetParent (transform);

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

	public void HostGame () {
		Manager.Init ();
		Network.HostGame ();
		Network.onUpdateClients += OnUpdateClients;
	}

	public void JoinGame (string hostId="") {
		Manager.Init ();
		if (hostId == "") {
			Network.JoinGame (Network.Hosts[0]);
		} else {
			Network.JoinGame (hostId);
		}
	}

	void OnUpdateClients (List<string> clients) {
		Manager.UpdatePeers (clients);
	}

	void OnReceiveMessage (NetworkMessage msg) {
		Debug.Log (Name + " ::: " + msg.id);
	}
}
