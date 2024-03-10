using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAutomata : MonoBehaviour
{
    [SerializeField] private int mapSaveIndex = 0;

    private int[,] terrainMap;
    [SerializeField] private Vector3Int tmpSize;
    [SerializeField] private TextAsset mapData;
    [Header("Map")]
    [SerializeField] private Tilemap topMap;
    [SerializeField] private Tilemap botMap;
    [SerializeField] private RuleTile topTile;
    [SerializeField] private Tile botTile;
    [Header("Minimap")]
    [SerializeField] private Tilemap topMinimap;
    [SerializeField] private Tilemap botMinimap;
    [SerializeField] private Tile topMinimapTile;
    [SerializeField] private Tile botMinimapTile;
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
                    topMap.SetTile(new Vector3Int(-x + width / 2 - 1, -y + height / 2  - 1, 0), topTile);
                    topMinimap.SetTile(new Vector3Int(-x + width / 2 - 1, -y + height / 2  - 1, 0), topMinimapTile);
                }
                else if (terrainMap[x, y] == 1)
                {
                    botMap.SetTile(new Vector3Int(-x + width / 2  - 1, -y + height / 2  - 1, 0), botTile);
                    botMinimap.SetTile(new Vector3Int(-x + width / 2  - 1, -y + height / 2  - 1, 0), botMinimapTile);
                }
            }
        }
    }
    
    public void SaveAssetMap()
    {
        string saveName = "Level" + mapSaveIndex;
        var mf = GameObject.Find("Grid");
        
        

        /*if (mf)
        {
            var savePath = "Assets/Prefabs/Maps/" + saveName + ".prefab";
            if (PrefabUtility.SaveAsPrefabAsset(mf, savePath))
            {
                EditorUtility.DisplayDialog("Tilemap saved", "Your Tilemap was saved under" + savePath, "Continue");
            }
            else
            {
                EditorUtility.DisplayDialog("Tilemap NOT saved",
                    "An ERROR occured while trying to saveTilemap under" + savePath, "Continue");
            }
        }*/
    }

    public void ClearMap(bool complete)
    {
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();
        botMinimap.ClearAllTiles();
        topMinimap.ClearAllTiles();
        if (complete)
        {
            terrainMap = null;
        }
    }
}