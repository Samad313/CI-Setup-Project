using JetBrains.Annotations;
using UnityEditor;


public static class VersioningMaintainer 
{
    static string androidVersion = "0.0.27";
    static string iOSVersion = "0.0.27";

    static string buildNumber = "4";
    static int androidVersionCode = 4;
    static BuildTarget buildTarget;



    [UsedImplicitly]
    public static void VersionSetter()
    {
        // Set version for this build

#if UNITY_IOS

        PlayerSettings.bundleVersion = iOSVersion;
        PlayerSettings.iOS.buildNumber = buildNumber;
        
#elif UNITY_ANDROID

        PlayerSettings.bundleVersion = androidVersion;
        PlayerSettings.Android.bundleVersionCode = androidVersionCode;

#elif UNITY_MAC

        PlayerSettings.macOS.buildNumber = buildNumber;

#endif

    }

}
