using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainImport : MonoBehaviour
{
    public int scale = 10;
    public string mapName;
    //public GameObject cursorObject;
    //public string[] TerrainTextures;
    //private Texture2D[] TerrainTexturesArray;
    //public Texture2D[] TerrainTextures = new Texture2D[8];


}

[CustomEditor(typeof(TerrainImport))]
public class TerrainImportEditor : Editor
{
    private TERRAIN_DATA std;
    private const int CHUNK_SIZE = 128;
    private int scale;
    private string mapName;

    

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TerrainImport myScript = (TerrainImport)target;
        scale = myScript.scale;
        mapName = myScript.mapName;
        if (GUILayout.Button("Generate"))
        {

            GenerateTerrain();
        }

        if (GUILayout.Button("Generate hmap texture"))
        {

            GenerateHMap();
        }
        if (GUILayout.Button("Generate splatmap texture"))
        {
            GenerateSplatMap();
        }
    }

    private void GenerateSplatMap()
    {
        std = new TERRAIN_DATA(mapName);
        std.OpenHdr();
        std.OpenVb(true, false);

        //int sizeX = std.Header.SizeXBPages * TerrainDefs.VBOXES_PAGE_SIZE * TerrainDefs.BPAGE_IN_VBPAGES;
        //int sizeY = std.Header.SizeZBPages * TerrainDefs.VBOXES_PAGE_SIZE * TerrainDefs.BPAGE_IN_VBPAGES;
        int sizeX=std.VBoxes.pager.SizeX();
        int sizeY= std.VBoxes.pager.SizeZ();
        Texture2D res = new Texture2D(sizeX,sizeY);
        T_VBOX data;
        
        
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX ; x++)
            {
                data = std.VBoxes.pager.Get(x, y);

                float height = NormalizeHeight((short) (data.water_level * 10));
                Color pixelColor = new Color(height, 0, 1 - height);

                res.SetPixel(x, y, pixelColor);
            }
        }
        res.Apply();
        AssetDatabase.CreateAsset(res, "Assets/splatmap.asset");
    }
    private void GenerateHMap()
    {
        
        std = new TERRAIN_DATA(mapName);
        std.OpenHdr();
        std.OpenSQ(true, false);
        
        Texture2D res = new Texture2D(std.Header.SizeXBPages * TerrainDefs.BOXES_PAGE_SIZE * TerrainDefs.SQUARES_IN_BOX, std.Header.SizeZBPages * TerrainDefs.BOXES_PAGE_SIZE * TerrainDefs.SQUARES_IN_BOX);
        T_SQUARE data;
        for (int y = 0; y < std.Header.SizeZBPages * TerrainDefs.BOXES_PAGE_SIZE * TerrainDefs.SQUARES_IN_BOX; y++)
        {
            for (int x = 0; x < std.Header.SizeXBPages  * TerrainDefs.BOXES_PAGE_SIZE * TerrainDefs.SQUARES_IN_BOX; x++)
            {
                data = std.Squares.pager.Get(x, y);

                float height = NormalizeHeight(data.Height);
                Color pixelColor = new Color(height, 0, 1-height);

                res.SetPixel(x, y, pixelColor);
            }
        }
        res.Apply();
        AssetDatabase.CreateAsset(res, "Assets/heightmap.asset");
    }
    private void GenerateTerrain()
    {
        std = new TERRAIN_DATA(mapName);
        std.OpenHdr();
        string message = mapName + " Terrain size: " + std.Header.SizeXBPages * TerrainDefs.BOXES_PAGE_SIZE * TerrainDefs.SQUARES_IN_BOX + "x" + std.Header.SizeZBPages * TerrainDefs.BOXES_PAGE_SIZE * TerrainDefs.SQUARES_IN_BOX;
        Debug.Log(message);
        std.OpenSQ(true, false);

        GameObject world = new GameObject
        {
            name = "World"
        };
        GameObject chunk;
        for (int y = 0; y < std.Header.SizeZBPages; y++)
        {
            for (int x = 0; x < std.Header.SizeXBPages; x++)
            {
                chunk = LoadChunk(x, y, CHUNK_SIZE);
                chunk.transform.parent = world.transform;
            }
        }
    }

    private GameObject LoadChunk(int startX, int startY, int size)
    {
        TerrainData terrainData = new TerrainData();
        terrainData.baseMapResolution = CHUNK_SIZE;
        terrainData.heightmapResolution = size + 1;
        terrainData.alphamapResolution = size + 1;
        terrainData.SetDetailResolution(1024, CHUNK_SIZE);
        terrainData.name = "TDATA " + startX + " " + startY;
        terrainData.size = new Vector3(size * scale, size * scale, size * scale);

        UpdateChunk(terrainData, startX, startY, size);
        GameObject terrain = Terrain.CreateTerrainGameObject(terrainData);
        terrain.name = "Terrain " + startX + " " + startY;

        terrain.transform.position = new Vector3(startX * size*scale, (0 - terrainData.size.y / 2), startY * size * scale); //set terrain on default water level
        //terrain.transform.parent = transform;

        return terrain;
    }


    private struct ChunkData
    {
        public float[,] sectorHeightMap;
        //public int[,] terrainTypes;
        //public int[] terrainTypesEnum;
    }

    private void UpdateChunk(TerrainData terrainData, int startX, int startY, int size)
    {
        T_SQUARE data;
        float[,] sectorHeightMap = new float[size + 1, size + 1];
        int[,] splatmap = new int[size + 1, size + 1];
        Debug.Log("Loading " + startX + "x" + startY);
        for (int y = 0; y < size + 1; y++)
        {
            for (int x = 0; x < size + 1; x++)
            {
                data = std.Squares.pager.Get(startX*size + x, startY*size + y);

                sectorHeightMap[y, x] = NormalizeHeight(data.Height);
                splatmap[x, y] = data.Flag & TerrainDefs.SQF_GRMASK;
            }
        }
        //TerrainData terrainData = terrain.GetComponent<Terrain>().terrainData;
        terrainData.name = "TDATA " + startX + " " + startY;
        terrainData.SetHeights(0, 0, sectorHeightMap);

        //terrain.name = "Terrain " + startX + " " + startY;
    }

    private float NormalizeHeight(short height)
    {
        //        short minHeight = -32768; //min short value
        //      short maxHeight = 32767;//max short value
        short minHeight = short.MinValue;
        short maxHeight = short.MaxValue;

        float NormalizedHeight = (float)((height - minHeight) / (float)(maxHeight - minHeight));

        return NormalizedHeight;
    }
}



