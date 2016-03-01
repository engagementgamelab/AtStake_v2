using System.Collections;
using System.Collections.Generic;

public interface IConnectionManager {
	ConnectionStatus Status { get; }
	void Host (string gameInstanceName);
	void Join (string hostName, string gameInstanceName);
	List<string> UpdateHosts ();
	void ConnectClient (string gameInstanceName);
	void DisconnectClient (string gameInstanceName);
	void Disconnect ();
}