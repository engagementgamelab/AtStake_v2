using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Reflection;
using JsonFx.Json;
using Models;

public class DataManager {
    
    /// <summary>
    /// Get the current API authentication key.
    /// </summary>
    public static string APIKey {
        get {
            return currentConfig.authKey;
        }
    }
    
    /// <summary>
    /// Get the current remote server URL.
    /// </summary>
    public static string RemoteURL {
        get {
            return currentConfig.root;
        }
    }

    /// <summary>
    /// Get the socket address to send & receive messages from
    /// </summary>
    public static string SocketAddress {
        get {
            return currentConfig.socket;
        }
    }

    public static GameEnvironment currentConfig;

    static JsonReaderSettings _readerSettings = new JsonReaderSettings();
    static GameConfig config;
    static GameData gameData;

    /// <summary>
    /// Set global game config data, such as API endpoints, given a valid input string
    /// </summary>
    /// <param name="data">Data to be used to set config; must conform to GameConfig model.</param>
    /// <param name="configTypeOverride">May be used to override config type in editor only (development, staging, production).</param>
    public static void SetGameConfig(string data, string configTypeOverride=null) {

        // Set config only if there is none set
        if(config != null) 
            return;

        // Set global config
        config = JsonReader.Deserialize<GameConfig>(data);
        
        // Set the current game config based on the environment
        #if UNITY_EDITOR
        
            currentConfig = config.local;

            // If override set, use that
            if(configTypeOverride != null) {
                if(configTypeOverride == "development")
                    currentConfig = config.development;
                else if(configTypeOverride == "staging")
                    currentConfig = config.staging;
                else if(configTypeOverride == "production")
                    currentConfig = config.production;
            }


        #elif DEVELOPMENT_BUILD
           currentConfig = config.development;
        #elif IS_PRODUCTION
           currentConfig = config.production;
        #else
           currentConfig = config.staging;
        #endif

    }

    /// <summary>
    /// Set global game data, given a valid input string
    /// </summary>
    /// <param name="data">String to be used to set game data; must conform to GameData model.</param>
    public static void SetGameData(string data) {

        try {
            JsonReader reader = new JsonReader(data, _readerSettings);
            gameData = reader.Deserialize<GameData>();
        }
        catch(JsonDeserializationException e) {
            throw new Exception("Unable to set game data: " + e.Message);
        }

        SaveDataToJson("data", data);
    }

    /// <summary>
    /// Save a string to specified JSON file in /Assets/Resources/ or the persistent data path for the app
    /// </summary>
    /// <param name="fileName">The file's name.</param>
    /// <param name="data">String to be used to save in JSON file.</param>
    /// <param name="persistentPath">Use the application persistent resource path?</param>
    public static void SaveDataToJson(string fileName, string data) {
        
        DirectoryInfo dirData = GetInfoAtPath (fileName);
        if(!dirData.Exists)
            dirData.Create();

        using (StreamWriter outfile = new StreamWriter(dirData.ToString() + fileName + ".json"))
        {
            outfile.Write(data);
        }

    }

    /// <summary>
    /// Gets the DirectoryInfo for the file with the given filename. Checks the appropriate path based on the platform.
    /// </summary>
    /// <param name="filename">Name of file to get info on</param>
    /// <returns>DirectoryInfo for the requested file</returns>
    public static DirectoryInfo GetInfoAtPath (string filename) {

        bool persistentPath = 
        #if !UNITY_EDITOR && UNITY_IOS || UNITY_ANDROID
        true
        #else
        false
        #endif
        ;

        string dataPath = (persistentPath ? Application.persistentDataPath : Application.dataPath) + "/Resources/";
        DirectoryInfo dirData = new DirectoryInfo(dataPath);
        dirData.Refresh();
        return dirData;
    }

    /// <summary>
    /// Returns true if the file with the given file name exists
    /// </summary>
    /// <param name="filename">Name of the file to check</param>
    /// <returns>True if the file exists, false otherwise</returns>
    public static bool FileExists (string filename) {
        return File.Exists (GetInfoAtPath (filename) + filename + ".json");
    }

    /// <summary>
    /// Loads the given file as a JSON file and returns the data as a string
    /// </summary>
    /// <param name="filename">Name of JSON file to open</param>
    /// <returns>A string in JSON format</returns>
    public static string LoadJsonData (string filename) {

        string localData;

        TextAsset dataJson = (TextAsset)Resources.Load (filename, typeof(TextAsset));
        StringReader strData = new StringReader (dataJson.text);
        localData = strData.ReadToEnd();
        strData.Close();

        return localData;
    }

    /// <summary>
    /// Deletes the file with the given name
    /// </summary>
    /// <param name="filename">Name of file to delete</param>
    public static void DeleteData (string filename) {
        DirectoryInfo info = GetInfoAtPath (filename);
        string path = info + filename + ".json";
        if (File.Exists (path)) {
            File.Delete (path);
            File.Delete (path + ".meta");
            info.Refresh ();
        }
    }

    /// <summary>
    /// Gets the game settings
    /// </summary>
    /// <returns>Settings data model</returns>
    public static Settings GetSettings () {
        return gameData.Settings;
    }

    /// <summary>
    /// Gets the available decks that can be played
    /// </summary>
    /// <returns>An array of Deck data models</returns>
    public static Deck[] GetDecks () {
        return gameData.Decks;
    }

    /// <summary>
    /// Finds the data model with the given symbol
    /// </summary>
    /// <param name="symbol">Symbol to search for</param>
    /// <returns>A Screen data model</returns>
    public static Models.Screen GetScreen (string symbol) {
        try {
            return System.Array.Find (gameData.Screens, x => x.Symbol == symbol);
        } catch {
            throw new System.Exception ("No screen with the symbol '" + symbol + "' could be found.");
        }
    }

    /// <summary>
    /// Gets text from the screen model based on its key. Also optionally replaces {{keywords}}
    /// </summary>
    /// <param name="screen">The Screen model to get text from</param>
    /// <param name="key">The text key to search for</param>
    /// <param name="vars">(optional) Keyword replacements. This should be in the format [keyword, replacement]</param>
    /// <returns>The requested text</returns>
    public static string GetTextFromScreen (Models.Screen screen, string key, Dictionary<string, string> vars=null) {
        string text;
        try {
            switch (key) {
                case "Instructions": text = screen.Instructions; break;
                case "DeciderInstructions": text = screen.DeciderInstructions; break;
                case "DeciderInstructionsOutLoud": text = screen.DeciderInstructionsOutLoud; break;
                case "PlayerInstructions": text = screen.PlayerInstructions; break;
                case "HostInstructions": text = screen.HostInstructions; break;
                case "ClientInstructions": text = screen.ClientInstructions; break;
                default: text = screen.Text[key]; break;
            }
        } catch {
            throw new System.Exception ("The screen " + screen.Symbol + " does not contain text with the key '" + key + "'");
        }

        // Match and replace keywords
        if (vars != null) {
            foreach (Match match in Regex.Matches (text, @"\{\{(.*?)\}\}")) {
                string m = match.Value;
                string v = m.Replace ("{{", "").Replace ("}}", "");
                string replace;
                if (vars.TryGetValue (v, out replace)) {
                    text = text.Replace (m, replace);
                }
            }
        }
        return text;
    }
}