using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.Serialization;

public class TileAutomata : MonoBehaviour
{
    [SerializeField] private int mapSaveIndex = 0;

    private int[,] terrainMap;
    [SerializeField] private Vector3Int tmpSize;
    [SerializeField] private Tilemap topMap;
    [SerializeField] private Tilemap botMap;
    [SerializeField] private TerrainTile topTile;
    [SerializeField] private AnimatedTile botTile;
    [SerializeField] private TextAsset mapData;

    int width;
    int height;

    public void CreateTileMap()
    {
        ClearMap(false);
        width = tmpSize.x;
        height = tmpSize.y;

        terrainMap = new int[width, height];
        string[] cellsData = mapData.text.Split("\r\n");
        for (int i = 0; i < cellsData.Length; i++)
        {
            if (cellsData[i] == "") continue;
            string[] cellData = cellsData[i].Split(',');
            terrainMap[int.Parse(cellData[0]), int.Parse(cellData[1])] = int.Parse(cellData[2]);
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 0)
                {
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), topTile);
                    //botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), botTile);
                }
            }
        }
    }
    
    public void SaveAssetMap()
    {
        string saveName = "tmapXY_" + mapSaveIndex;
        var mf = GameObject.Find("Grid");

        if (mf)
        {
            var savePath = "Assets/Prefabs/Maps/" + saveName + ".prefab";
            if (PrefabUtility.CreatePrefab(savePath, mf))
            {
                EditorUtility.DisplayDialog("Tilemap saved", "Your Tilemap was saved under" + savePath, "Continue");
            }
            else
            {
                EditorUtility.DisplayDialog("Tilemap NOT saved",
                    "An ERROR occured while trying to saveTilemap under" + savePath, "Continue");
            }
        }
    }

    public void ClearMap(bool complete)
    {
        topMap.ClearAllTiles();
        //botMap.ClearAllTiles();
        if (complete)
        {
            terrainMap = null;
        }
    }
}