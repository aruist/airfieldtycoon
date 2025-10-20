using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[Serializable]
public class SCCryptListItem
{
    [SerializeField]
    public string m_Name = "ab";
    [SerializeField]
    public string m_Comment = "comment";
    [SerializeField]
    public string m_Value = "cd";


    /*public void drawCustomGUI()
    {
        m_Name = EditorGUILayout.TextField("Name", m_Name);
        m_Value = EditorGUILayout.TextField("Value", m_Value);
    }*/

}
