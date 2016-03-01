using UnityEngine;
using System.Collections;

public class BluetoothManager : MonoBehaviour, IConnectionManager {

	public ConnectionStatus Status {
		get { return status; }
	}

	ConnectionStatus status;

	public void Host (string gameInstanceName) {

	}
}
