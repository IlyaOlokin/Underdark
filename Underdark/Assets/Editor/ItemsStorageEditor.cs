using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemsStorageSO))]
public class ItemsStorageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ItemsStorageSO itemsStorage = (ItemsStorageSO)target;
        
        if(GUILayout.Button("Load Items"))
            itemsStorage.LoadItems();
        
        if(GUILayout.Button("Clear"))
            itemsStorage.Clear();
        
    }
}
