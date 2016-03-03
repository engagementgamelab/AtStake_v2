using UnityEngine;
using System.Collections;

// public enum ConnectionStatus { Undetermined, Failed, Succeeded }

public class NetworkConnectionTest : MonoBehaviour {

	/*ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;

	string prevTestMessage = "";
	ConnectionStatus testResult = ConnectionStatus.Undetermined;
	NetworkingManager.Settings settings;
	System.Action<ConnectionStatus> onClientResults;

	public void Init (NetworkingManager.Settings settings) {
		this.settings = settings;
	}

	// Client test

	public void TestClientConnection (System.Action<ConnectionStatus> onClientResults) {
		this.onClientResults = onClientResults;
		Debug.Log ("Testing connection to multiplayer server");
		Network.InitializeServer (settings.MaxConnections, 25001, !Network.HavePublicAddress ());
	}

	void OnServerInitialized () {
		if (testResult == ConnectionStatus.Undetermined) {
			Debug.Log ("Multiplayer server initiated");
			MasterServer.RegisterHost (settings.GameName, "TestConnection");
		}
	}

	void OnFailedToConnectToMasterServer (NetworkConnectionError err) {
		if (testResult == ConnectionStatus.Undetermined) {
			testResult = ConnectionStatus.Failed;
			Debug.LogWarning ("Failed to connect to multiplayer server");
			onClientResults (testResult);
		}
	}

	void OnMasterServerEvent (MasterServerEvent e) {
		if (e == MasterServerEvent.RegistrationSucceeded) {
			testResult = ConnectionStatus.Succeeded;
			Network.Disconnect ();
			Debug.Log ("Succeeded connecting to multiplayer server");
			onClientResults (testResult);
		}
	}

	// Server test

	public void TestServerConnection (System.Action<ConnectionStatus> onServerResults) {
		StartCoroutine (CoTestServerConnection (onServerResults));
	}

	IEnumerator CoTestServerConnection (System.Action<ConnectionStatus> onServerResults) {

		bool useNat = false;
		bool probingPublicIP = false;
		float timer = 0;

		Network.InitializeServer (settings.MaxConnections, 25001, !Network.HavePublicAddress ());

		while (testResult == ConnectionStatus.Undetermined) {
			
			connectionTestResult = Network.TestConnection();

			switch (connectionTestResult) {

				case ConnectionTesterStatus.Error: 
					LogTestMessage ("Problem determining NAT capabilities", true);
					testResult = ConnectionStatus.Failed;
					break;
						
				case ConnectionTesterStatus.Undetermined: 
					LogTestMessage ("Undetermined NAT capabilities", true);
					testResult = ConnectionStatus.Undetermined;
					break;
									
				case ConnectionTesterStatus.PublicIPIsConnectable:
					LogTestMessage ("Directly connectable public IP address.");
					useNat = false;
					testResult = ConnectionStatus.Succeeded;
					break;
						
				// This case is a bit special as we now need to check if we can 
				// circumvent the blocking by using NAT punchthrough
				case ConnectionTesterStatus.PublicIPPortBlocked:
					LogTestMessage ("Non-connectable public IP address (port " +
						DataManager.MultiplayerServerPort +" blocked), running a server is impossible.", true);
					useNat = false;
					// If no NAT punchthrough test has been performed on this public 
					// IP, force a test
					if (!probingPublicIP) {
						connectionTestResult = Network.TestConnectionNAT ();
						probingPublicIP = true;
						LogTestMessage ("Testing if blocked public IP can be circumvented");
						timer = Time.time + 10;
					}
					// NAT punchthrough test was performed but we still get blocked
					else if (Time.time > timer) {
						probingPublicIP = false; // reset
						useNat = true;
						testResult = ConnectionStatus.Succeeded; // failed? 
					}
					break;
					
				case ConnectionTesterStatus.PublicIPNoServerStarted:
					LogTestMessage ("Public IP address but server not initialized, "+
						"it must be started to check server accessibility. Restart "+
						"connection test when ready.", true);
					break;
									
				case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
					LogTestMessage ("Limited NAT punchthrough capabilities. Cannot "+
						"connect to all types of NAT servers. Running a server "+
						"is ill advised as not everyone can connect.", true);
					useNat = true;
					testResult = ConnectionStatus.Succeeded;
					break;
						
				case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
					LogTestMessage ("Limited NAT punchthrough capabilities. Cannot "+
						"connect to all types of NAT servers. Running a server "+
						"is ill advised as not everyone can connect.", true);
					useNat = true;
					testResult = ConnectionStatus.Succeeded;
					break;
					
				case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
				case ConnectionTesterStatus.NATpunchthroughFullCone:
					LogTestMessage ("NAT punchthrough capable. Can connect to all "+
						"servers and receive connections from all clients. Enabling "+
						"NAT punchthrough functionality.");
					useNat = true;
					testResult = ConnectionStatus.Succeeded;
					break;
			
				default: 
					Debug.LogError ("Error in test routine, got " + connectionTestResult);
					break;
			}
			
			yield return null;
		}

		if (useNat)
			LogTestMessage ("When starting a server the NAT "+
				"punchthrough feature should be enabled (useNat parameter)");
		else
			LogTestMessage ("NAT punchthrough not needed");
		LogTestMessage ("Done testing");

		Debug.Log (testResult);

		Network.Disconnect ();

		onServerResults (testResult);
	}

	void LogTestMessage (string msg, bool warning=false) {
		if (msg != prevTestMessage) {
			if (warning)
				Debug.LogWarning (msg);
			else
				Debug.Log (msg);
			prevTestMessage = msg;
		}
	}*/
}
