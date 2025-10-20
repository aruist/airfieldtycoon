using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class SCCryptBuilderWindow : EditorWindow {
    private SCCryptData m_cryptData;

    [MenuItem("Window/Softcen/Crypt Builder")]
    static void Init()
    {
        SCCryptBuilderWindow window = (SCCryptBuilderWindow)EditorWindow.GetWindow(typeof(SCCryptBuilderWindow));
        window.Show();
        //GetWindow<CryptTextWindow>();
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.HideAndDontSave;

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
			assignEditorWindowIcon(this, getIconPath(getProductName()), getProductName());
#endif

        init();
    }

    public void init()
    {
        loadScriptableObject();
    }

    private Vector2 m_scrollPosition;

    public void OnGUI()
    {
        m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
        {
            GUILayout.Space(5f);
            m_cryptData.m_Filename = EditorGUILayout.TextField("Filename", m_cryptData.m_Filename);
            m_cryptData.m_ClassName = EditorGUILayout.TextField("Class name", m_cryptData.m_ClassName);

            List<SCCryptListItem> cryptList = m_cryptData.getList;
            if (cryptList != null)
            {
                GUILayout.Space(10f);
                foreach (var instance in cryptList)
                {
                    SCCryptListItem li = instance;
                    GUILayout.Space(5f);
                    li.m_Name = EditorGUILayout.TextField("Name", li.m_Name);
                    li.m_Comment = EditorGUILayout.TextField("Comment", li.m_Comment);
                    li.m_Value = EditorGUILayout.TextField("Value", li.m_Value);

                }
                GUILayout.Space(10f);
                if (GUILayout.Button("Add new item"))
                {
                    cryptList.Add(new SCCryptListItem());
                }

            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_cryptData);
                //EditorUtility.SetDirty(m_cryptData.m_ListItemOne);
            }

            GUILayout.Space(5f);
            GUILayout.Space(10f);
            if (GUILayout.Button("Save"))
            {
                UnityEditor.EditorUtility.SetDirty(m_cryptData);
                //UnityEditor.EditorUtility.SetDirty(m_cryptData.ListItemOne);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }
            GUILayout.Space(10f);
            if (GUILayout.Button("Generate File"))
            {
                GenerateFile();
            }

        }
        EditorGUILayout.EndScrollView();

    }

    private void loadScriptableObject()
    {
        m_cryptData = SCEditUtils.CreateScriptableObject<SCCryptData>("Assets/Softcen/CryptBuilder/Editor/SCCryptBuilder_data.asset");
    }

    private void GenerateFile()
    {
        string filename = Application.dataPath + "/" + m_cryptData.m_Filename;
        Debug.Log("Start Generating file: " + filename);
        try
        {
            StreamWriter sw = File.CreateText(filename);
            sw.Write("using UnityEngine;\n\n");
            sw.Write("public class " + m_cryptData.m_ClassName + " {\n");
            sw.Write("    public class L_23sd {\n");
            if (m_cryptData != null)
            {
                sw.Write("        // Level data generated. Do not change!!!\n");
                List<SCCryptListItem> cryptList = m_cryptData.getList;
                foreach (var instance in cryptList)
                {
                    sw.Write("        // " + instance.m_Comment + "\n");
                    sw.Write("        public static int[] " + instance.m_Name + " = {\n");
                    //Debug.Log("Value: " + instance.m_Value);
                    string encStr = Encrypt.EncryptString(instance.m_Value);
                    string numbers = "            ";
                    for (int i = 0; i < encStr.Length; i++)
                    {
                        int num = encStr[i];
                        numbers += num.ToString() + ",";
                        if (i != 0 && i % 40 == 39)
                        {
                            numbers += "\n            ";
                        }
                    }
                    sw.Write(numbers);
                    sw.Write("\n        };\n\n");
                }
            }
            sw.Write("    }\n");
            sw.Write("}\n");
            sw.Close();

        }
        catch
        {
            Debug.LogError("File geneartion failde!");
        }
        Debug.Log("End Generating file " + filename);

    }
}
