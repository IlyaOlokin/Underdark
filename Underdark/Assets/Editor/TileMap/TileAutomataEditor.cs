using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(TileAutomata))]
public class TileAutomataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TileAutomata tileAutomata = (TileAutomata)target;
        
        if(GUILayout.Button("Generate Terrain"))
            tileAutomata.CreateTileMap();
        
        if(GUILayout.Button("Clear Terrain"))
            tileAutomata.ClearMap(true);
        
        if(GUILayout.Button("Save Terrain"))
            tileAutomata.SaveAssetMap();
    }
}
