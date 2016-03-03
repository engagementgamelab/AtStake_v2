using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using JsonFx.Json;
 
public class ApiManager : MonoBehaviour {

    protected ApiManager() {}
    private static ApiManager _instance = null;

    private static Action<Dictionary<string, object>> _currentResponseHandler;
    public static string _sessionCookie;
    public static string _userCookie;
    
    public delegate void OnServerDown();
    public delegate void OnNotLoggedIn();
    public delegate void OnNoNetwork(bool connectionNotFound);

    float _elapsedRequestTime = 0;
    float _timeoutCap = 5;
    
    bool _hasRequest;
    bool _ignoreNetwork;
    bool _isAuthenticated;

    IEnumerator _currentRoutine;
    WWW _wwwRequest;

    class PostCache {
        
        public string url;
        public Dictionary<string, object> fields;
        public Action<Dictionary<string, object>> responseHandler;

    }
    List<PostCache> _cachedRequests = new List<PostCache>();
        
    public static ApiManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType (typeof (ApiManager)) as ApiManager;

                if (_instance == null) {
                    GameObject obj = new GameObject ();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<ApiManager> ();
                }
            }
            return _instance;
        }
    }

    public string Cookie {
        
        set {
            _sessionCookie = value;

            // Client was authenticated -- call any requests that were waiting
            foreach(PostCache req in _cachedRequests)
                PostURL(req.url, req.fields, req.responseHandler);

            _cachedRequests.Clear();
        }

    }

    public static Action<Dictionary<string, object>> CurrentResponseHandler {
        
        get {
            return _currentResponseHandler;
        }
        set {
            _currentResponseHandler = value;
        }

    }

    /// <summary>
    /// Called when there is a server error.
    /// </summary>
    public OnServerDown onServerDown;

    /// <summary>
    /// Called when the user has no session.
    /// </summary>
    public OnNotLoggedIn onNotLoggedIn;

    /// <summary>
    /// Called when the user has no network connection.
    /// </summary>
    public OnNoNetwork onNoNetwork;

    /// <summary>
    /// Is API authenticated?
    /// </summary>
    /// <returns>Yes or no.</returns>
    public bool Authenticated {
        get {
            return _isAuthenticated;
        }
        set {
            _isAuthenticated = value;
        }
    }

    public void Authenticate(Action<Dictionary<string, object>> responseHandler=null) {

        // Bail if network not used
        if(_ignoreNetwork) 
            return;

        PostURL(
            "/auth/",
            new Dictionary<string, object>() {{ "key", DataManager.APIKey }},
            responseHandler
        );

    }
    
    /// <summary>
    /// Calls a URL endpoint and gives an optional response handler.
    /// </summary>
    /// <param name="url">The URL to get.</param>
    /// <param name="responseHandler">An Action that handles the response (optional).</param>
    public void GetURL(string url, Action<string> responseHandler=null) {

        string absoluteURL = DataManager.RemoteURL + url;
        
        if(responseHandler == null)
            StartCoroutine(WaitForRequest(absoluteURL));
        else
            StartCoroutine(WaitForRequest(absoluteURL, responseHandler));

    }


    /// <summary>
    /// Post to a a URL endpoint with required field(s) and gives an optional response handler.
    /// </summary>
    /// <param name="url">The URL to post to.</param>
    /// <param name="responseHandler">An Action that handles the response (optional).</param>
    public void PostURL(string url, Dictionary<string, object> fields, Action<Dictionary<string, object>> responseHandler=null) {

        // Kill networking immediately if no connection
        // if(Application.internetReachability == NetworkReachability.NotReachable)
        //     KillNetwork(true);

        if(_ignoreNetwork) {
            fields.Add("local", true);
            responseHandler(fields);
            return;
        }

        string absoluteURL = DataManager.RemoteURL + url;

        // Send form as raw byte array
        System.Text.StringBuilder output = new System.Text.StringBuilder();
        
        JsonWriter writer = new JsonWriter (output);
        
        if(_sessionCookie != null)
            fields.Add("sessionID", System.Convert.ChangeType(_sessionCookie, typeof(object)));
        
        // If no session cookie, client has no auth, so cache this request to do later
        else if(url != "/auth/") {
            PostCache cacheObj = new PostCache();
            cacheObj.url = url;
            cacheObj.fields = fields;
            cacheObj.responseHandler = responseHandler;
            _cachedRequests.Add(cacheObj);

            return;
        }

        writer.Write(fields);
     
        // Encode output as UTF8 bytes
        _currentRoutine = WaitForForm(absoluteURL, Encoding.UTF8.GetBytes(output.ToString()), responseHandler);
        StartCoroutine(_currentRoutine);
    
    }

    /// <summary>
    /// Download data from specified URL.
    /// </summary>
    /// <param name="url">The URL to download from.</param>
    public string DownloadDataFromURL(string url) {

        WebClient client = new WebClient();

        try {
            
            Stream data = client.OpenRead(DataManager.RemoteURL + url);

            StreamReader reader = new StreamReader(data);
            
            return reader.ReadToEnd();

        }
        catch(WebException e) {

            throw new Exception("Unable to download data from " + url + ": " + e.Message);

        }
    }

    IEnumerator WaitForRequest(string url, Action<string> responseAction=null)
     {        
        WWW wwwGet = new WWW(url);

        yield return wwwGet;

        // Check for errors
        if (wwwGet.error == null) 
        {
            if(responseAction != null)
            {
                // Respond w/ error text
                responseAction(wwwGet.text);
                yield return null;
            }
        }
        else
        {
            string exceptionMsg = "WaitForRequest error: " + wwwGet.error;
            
            throw new Exception(exceptionMsg);
        }
     }

    IEnumerator WaitForForm(string url, byte[] formData=null, Action<Dictionary<string, object>> responseAction=null, WWWForm form=null)
     {
        // Specified raw form as JSON
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");

        // This is a raw UTF8 form
        if(form == null && formData != null) {
        
            if(_sessionCookie != null)
                postHeader.Add("x-sessionID", _sessionCookie);
        
            using(_wwwRequest = new WWW(url, formData, postHeader)) {
            
                _hasRequest = true;
                _elapsedRequestTime = 0;

                yield return _wwwRequest;

                _hasRequest = false;

                if(_wwwRequest == null)
                    yield return null;

                // Debug.Log(_wwwRequest.text);

                // Deserialize the response and handle it below
                Dictionary<string, object> response = JsonReader.Deserialize<Dictionary<string, object>>(_wwwRequest.text);

                // User is not logged in
                if((_wwwRequest.responseHeaders.Count > 0) && _wwwRequest.responseHeaders.ContainsKey("STATUS") && _wwwRequest.responseHeaders["STATUS"].ToString().Contains("401"))  
                    onNotLoggedIn();

                // check for errors
                else if (_wwwRequest.error == null) 
                {

                    if(responseAction != null)
                    {
                        responseAction(response);
                        yield return null;
                    }

                }

                else
                {
                    string exceptionMsg = "WaitForForm unknown error. No response to parse and no registered callback.";

                    if(response == null)
                    {

                        // If in editor, always throw so we catch issues
                        #if UNITY_EDITOR || DEVELOPMENT_BUILD
                            exceptionMsg = "General _wwwRequest issue: " + _wwwRequest.error;
                            Debug.Log(exceptionMsg);
                            
                            // Kill all networking
                            KillNetwork(false);
                        #endif
                        
                        if(!_ignoreNetwork) {
                        
                            if(_wwwRequest.error.Equals("couldn't connect to host")) {
                                // Kill all networking
                                KillNetwork(false);
                            }
                            else if(_wwwRequest.error.StartsWith("Connection timed out") || _wwwRequest.error.StartsWith("Couldn't resolve host")) {
                                // Timed out, kill due to slow connection
                                KillNetwork(true);
                            }

                        }

                    }
                    else if(responseAction != null && !response.ContainsKey("error")) 
                    {
                        responseAction(response);
                        yield return null;
                    }
                    else
                    {
                        responseAction (response);
                    }
                    
                }
            }
        }
        
        // Bail out!
        else
            throw new Exception("WaitForForm: both form and form byte data not specified.");

     }

    #if !UNITY_WEBGL
    void Update () {
        
        if(_hasRequest) {
            _elapsedRequestTime += Time.deltaTime;
            // Debug.Log (_elapsedRequestTime);
        }

        // Cease any request if takes too long and murder networking
        if(_elapsedRequestTime >= _timeoutCap && _hasRequest){
            StopCoroutine (_currentRoutine);
        
            KillNetwork (false);
        }
    
    }
    #endif

    void KillNetwork(bool noConnection) {

        if(_ignoreNetwork)
            return;
        
        Debug.Log(">>>> Could not connect to EL API - Using local data <<<<");

        // Toss the request
        if(_wwwRequest != null)     
            _wwwRequest.Dispose();

        // Call delegate
        if (onNoNetwork != null)
            onNoNetwork(noConnection);

        _ignoreNetwork = true;
        _hasRequest = false;

    }

}