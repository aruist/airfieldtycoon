#if UNITY_ANDROID
/*
using Google.JarResolver;
using UnityEditor;
[InitializeOnLoad]
public static class GooglePlayServicesDependencies
{
    /// The name of the plugin.
    private static readonly string PluginName = "PlayServices";
    static GooglePlayServicesDependencies()
    {
        PlayServicesSupport svcSupport =
            PlayServicesSupport.CreateInstance(PluginName, EditorPrefs.GetString("AndroidSdkRoot"),
                    "ProjectSettings");

        svcSupport.DependOn("com.google.android.gms", "play-services-games", "LATEST");
        svcSupport.DependOn("com.google.android.gms", "play-services-plus", "LATEST");
        svcSupport.DependOn("com.google.android.gms", "play-services-auth", "LATEST");
        svcSupport.DependOn("com.google.android.gms", "play-services-nearby", "LATEST");

    }
}*/
#endif
