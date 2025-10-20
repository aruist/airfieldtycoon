#if !UNITY_IOS
//using Google.JarResolver;
using UnityEditor;

/// AdMob dependencies file.
[InitializeOnLoad]
public static class GoogleAnalyticsDependencies
{
    /// The name of the plugin.
    /*private static readonly string PluginName = "GoogleAnalyticsV4";

    /// Initializes static members of the class.
    static GoogleAnalyticsDependencies()
    {
        PlayServicesSupport svcSupport =
            PlayServicesSupport.CreateInstance(PluginName, EditorPrefs.GetString("AndroidSdkRoot"),
                    "ProjectSettings");

        svcSupport.DependOn("com.google.android.gms", "play-services-analytics", "LATEST");
    }*/
}
#endif
