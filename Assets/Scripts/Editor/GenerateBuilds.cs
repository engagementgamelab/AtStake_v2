#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class GenerateBuilds {

	static string APP_NAME = "AtStake";
    static string TARGET_DIR = "Builds";

    // Options for all builds
    static BuildOptions BUILD_OPTIONS = BuildOptions.Development | BuildOptions.AllowDebugging;

    [MenuItem ("Build/Build for Production")]
    static void MakeProductionBuilds () {
    	BUILD_OPTIONS = BuildOptions.None;
    	PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, "IS_PRODUCTION");
        PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, "IS_PRODUCTION");
    	MakeMobileBuilds ();
    }

    [MenuItem ("Build/Build All")]
    static void MakeMobileBuilds () {
        PerformiOSBuild ();
        PerformAndroidBuild ();
    }

    [MenuItem ("Build/Mobile/iOS")]
    static void PerformiOSBuild () {
        GenericBuild ("iOS", BuildTarget.iOS);
    }

    [MenuItem ("Build/Mobile/Android")]
    static void PerformAndroidBuild () {
        GenericBuild ("Android", BuildTarget.Android);
    }

    static string GetBuildDirectory (string platform) {
		return TARGET_DIR + "/" + platform + "/";
    }

	static void GenericBuild (string platform, BuildTarget buildTarget) {

		// Set target platform
		EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);

		// Disable splash and resolution screens
		PlayerSettings.showUnitySplashScreen = false;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;

        // Set product and company name
        PlayerSettings.productName = (buildTarget == BuildTarget.iOS) ? "@Stake" : "AtStake";
        PlayerSettings.companyName = "Engagement Lab";

        string res = BuildPipeline.BuildPlayer (new string[] { "Assets/Scenes/Main.unity" }, GetBuildDirectory (platform) + APP_NAME, buildTarget, BUILD_OPTIONS);

        if (res.Length > 0)
            throw new System.Exception("BuildPlayer failure: " + res);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, null);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, null);
	}
}
#endif