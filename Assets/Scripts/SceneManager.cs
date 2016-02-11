using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneManager : MonoBehaviour {

	public string environment;

	void Awake () { 

		NetworkManager.Instance.onServerDown += OnServerDown;

		// We need our game config data before calling any remote endpoints
		LoadGameConfig ();
			
		// Set global game data local fallback in case of no connection
		// SetGameData (true);
	}

	void Start() {

		// Set loading indicator styles
		#if UNITY_IPHONE
	        Handheld.SetActivityIndicatorStyle (UnityEngine.iOS.ActivityIndicatorStyle.WhiteLarge);
	    #elif UNITY_ANDROID
	        Handheld.SetActivityIndicatorStyle (AndroidActivityIndicatorStyle.Large);
	    #endif

		// Authenticate to API
	    if (!NetworkManager.Instance.Authenticated) {
	    	Debug.Log("Authenticate to API");
			NetworkManager.Instance.Authenticate (ClientAuthenticated);
	    }
	}

	#if UNITY_EDITOR
	void OnGUI() {
		/*GUIStyle style = new GUIStyle();
		
		style.fontSize = 13;
		style.fontStyle = FontStyle.BoldAndItalic;

	    GUI.contentColor = Color.white;

        GUI.Label(new Rect(4, 4, 100, 20), "ENVIRONMENT: " + environment, style);*/
	}
	#endif

	/// <summary>
	/// Client was authenticated to API; we can now get game data and ask player to log in
	/// </summary>
	/// <param name="response">Dictionary containing "authed" key telling us if API auth </param>
	public void ClientAuthenticated(Dictionary<string, object> response) {

		// If failed (bad auth or local)
		if (response.ContainsKey("local")) {
			Debug.Log("Unable to contact API endpoint.");
			return;
		}
		else if (!System.Convert.ToBoolean(response["authed"])) {
			Debug.Log(">>> API authentication failed!");
			return;	
		}

		NetworkManager.Instance.Cookie = response["session_cookie"].ToString();

		// Set as authenticated
		NetworkManager.Instance.Authenticated = true;

		// Set global game data if needed
		SetGameData ();
	}
	
	/// <summary>
	/// User attempted authentication; return/show error if failed
	/// </summary>
    /// <param name="success">Was authentication successful?.</param>
	public void UserAuthenticateResponse(bool success) {
		if (!success) return;
		Debug.Log("Player auth successful? " + success);
	}

	void OnServerDown() {
		Debug.LogWarning ("Show on server down");
	}

	/// <summary>
	/// Obtains game config data and passes it to global data manager
	/// </summary>
	void LoadGameConfig () {
		
		Debug.Log ("Loading game config");

		// Open stream to API JSON config file
		TextAsset apiJson = (TextAsset)Resources.Load ("api", typeof(TextAsset));
		StringReader strConfigData = new StringReader (apiJson.text);

		// Set in data manager class with chosen environment config
		DataManager.SetGameConfig (strConfigData.ReadToEnd (), environment);

		strConfigData.Close ();
	}

	/// <summary>
	/// Obtains and sets global game data
	/// </summary>
	void SetGameData (bool fallback=false) {

		string gameData;

		if (fallback)
			gameData = GameDataLoadFallback ();

		else
		{
			// This should live in a static global dictionary somewhere
			// Try to get data from API remote
			try {
				gameData = NetworkManager.Instance.DownloadDataFromURL("/gameData");
			}

			// Fallback: load game data from local config
			catch(System.Exception e) {
				gameData = GameDataLoadFallback ();
			}
		}

		GameDataResponse (gameData);

	}

	string GameDataLoadFallback () {

		string localData;

		TextAsset dataJson = (TextAsset)Resources.Load ("data", typeof(TextAsset));
		StringReader strData = new StringReader (dataJson.text);
		localData = strData.ReadToEnd();
		strData.Close();

		return localData;
	}

	void GameDataResponse(string data) {

		// Set global game data
		if (!string.IsNullOrEmpty (data))
			DataManager.SetGameData (data);
	}
}
