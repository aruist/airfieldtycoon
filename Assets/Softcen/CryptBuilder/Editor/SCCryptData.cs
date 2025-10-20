using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SCCryptData : ScriptableObject
{
    [SerializeField]
    public string m_Filename;
    [SerializeField]
    public string m_ClassName;
    [SerializeField]
    private List<SCCryptListItem> m_List = new List<SCCryptListItem>();
    public List<SCCryptListItem> getList {  get { return m_List; } }

    
}
