/*#if UNITY_ANDROID
using Google.JarResolver;
using UnityEditor;
[InitializeOnLoad]
public static class FirebaseDependencies
{
    /// The name of the plugin.
    private static readonly string PluginName = "Firebase";
    static FirebaseDependencies()
    {
        PlayServicesSupport svcSupport =
            PlayServicesSupport.CreateInstance(PluginName, EditorPrefs.GetString("AndroidSdkRoot"),
                    "ProjectSettings");

        svcSupport.DependOn("com.google.firebase", "firebase-core", "LATEST");

    }
}
#endif*/
