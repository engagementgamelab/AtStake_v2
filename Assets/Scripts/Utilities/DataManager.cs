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

    public static string MasterServerAddress {
        get {
            return currentConfig.master;
        }
    }

    public static string SocketAddress {
        get {
            return currentConfig.socket;
        }
    }

    /// <summary>
    /// Set to production mode.
    /// </summary>
    /*public static bool Production {
        set {
            isProduction = value;
        }
    }*/

    public static GameEnvironment currentConfig;

    static JsonReaderSettings _readerSettings = new JsonReaderSettings();
    static GameConfig config;
    static GameData gameData;
    // static bool isProduction;

    /*static Dictionary<string, string> localUIText = new Dictionary<string, string>() {
        {"copy_server_down_header", "Sorry!"},
        {"copy_server_down_body", "The game's server is currently unreachable. Your internet connection may be having some issues, or the server is offline for regular maintenance.\n\nPlease close the application and try again in a few minutes. Apologies for the inconvenience!"}
    };*/

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
        // #if UNITY_EDITOR
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


        /*#elif DEVELOPMENT_BUILD
           currentConfig = config.development;
        #elif IS_PRODUCTION
           currentConfig = config.production;
        #else
           currentConfig = config.staging;
        #endif*/

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

        // create/save to file in Assets/Resources/
        #if !UNITY_WEBPLAYER 

            SaveDataToJson("data", data
                #if !UNITY_EDITOR && UNITY_IOS || UNITY_ANDROID
                ,true
                #endif
            );

        #endif
    }

    /// <summary>
    /// Save a string to specified JSON file in /Assets/Resources/ or the persistent data path for the app
    /// </summary>
    /// <param name="fileName">The file's name.</param>
    /// <param name="data">String to be used to save in JSON file.</param>
    /// <param name="persistentPath">Use the application persistent resource path?</param>
    static void SaveDataToJson(string fileName, string data, bool persistentPath=false) {

        string dataPath = (persistentPath ? Application.persistentDataPath : Application.dataPath) + "/Resources/";
        DirectoryInfo dirData = new DirectoryInfo(dataPath);
        dirData.Refresh();
        
        if(!dirData.Exists)
            dirData.Create();

        using (StreamWriter outfile = new StreamWriter(dataPath + fileName + ".json"))
        {
            outfile.Write(data);
        }

    }

    public static Settings GetSettings () {
        return gameData.Settings;
    }

    public static Deck[] GetDecks () {
        return gameData.Decks;
    }

    public static string[] GetQuestions (string deckKey) {
        return System.Array.Find (GetDecks (), x => x.Name == deckKey).Questions;
    }

    public static Models.Screen GetScreen (string symbol) {
        try {
            return System.Array.Find (gameData.Screens, x => x.Symbol == symbol);
        } catch {
            throw new System.Exception ("No screen with the symbol '" + symbol + "' could be found.");
        }
    }

    public static string GetTextFromScreen (Models.Screen screen, string key, Dictionary<string, string> vars=null) {
        string text;
        try {
            switch (key) {
                case "Instructions": text = screen.Instructions; break;
                case "DeciderInstructions": text = screen.DeciderInstructions; break;
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