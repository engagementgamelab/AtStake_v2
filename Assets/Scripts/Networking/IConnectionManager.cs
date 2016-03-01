using UnityEngine;
using System.Collections;

public interface IConnectionManager {
	ConnectionStatus Status { get; }
	void Host (string gameInstanceName);
}