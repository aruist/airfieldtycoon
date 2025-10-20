using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

public static class SCEditUtils
{

    public static T CreateScriptableObject<T>(string path) where T : ScriptableObject
    {
        T scriptableObject = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));

        if (scriptableObject != null)
        {
            Debug.Log("CreateScriptableObject return .asset");
            return scriptableObject;
        }

        Debug.Log("CreateScriptableObject create new");
        scriptableObject = ScriptableObject.CreateInstance<T>();
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);

        string directoryPath = path.Substring(0, path.LastIndexOf("/"));
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            return null;
        }

        AssetDatabase.CreateAsset(scriptableObject, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("CreateScriptableObject create new: " + assetPathAndName);

        return scriptableObject;
    }

}
