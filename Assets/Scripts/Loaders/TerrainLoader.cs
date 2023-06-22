using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.IO;
using UnityEditor;

public class TerrainLoader : MonoBehaviour
{
    // Start is called before the first frame update
    [Tooltip("Terrains scale (how many units in per pixel")]
    public int scale = 10;
    [Tooltip("Terrain file name")]
    public string mapName;
    [Tooltip("Map (chunks) file name")]
    public string GameMapName;
    [Tooltip("Map (chunks) size")]
    public Vector2Int GameMapSize;
    private TERRAIN_DATA std;
    public GameObject cursorObject;
    private const int CHUNK_SIZE = 128;
    public Material flatland;
    private Transform FlatlandTransform;
    private List<SpoolChunkData> terrainPool;
    private List<SpoolChunkData> loadedChunks;
    public string[] TerrainTextures;
    public SurfData[] TerrainTextures1;
    private Texture2D[] TerrainTexturesArray;

    private Vector2Int prevCursorChunk;
    private bool IsFirstFrame;
    private GameObject World;

    public Vector2Int currentChunk;

    private BaseObject ObjData;
    private GameObject CurrentSectorCube;

    void Start()
    {
        Initialize();
        LoadPlayerArea();
    }

    private void FixedUpdate()
    {
        LoadPlayerArea();
        //ObjData.SetHUDLocalCoordinates(GetLocalChunkCoords());
    }

    private Vector3 GetLocalChunkCoords()
    {
        return new Vector3((float)ObjData.pos.x % (scale * CHUNK_SIZE), cursorObject.transform.position.y, (float)ObjData.pos.z % (scale * CHUNK_SIZE));
    }
    public void InitCursorObject()
    {
        if (!cursorObject.TryGetComponent<BaseObject>(out ObjData)) return;
    }
    private void InitTextures()
    {
        if (TerrainTextures.Length == 0) return;
        TerrainTexturesArray = new Texture2D[TerrainTextures.Length];
        Texture2D tmpTexture;
        for (int i = 0; i < TerrainTextures.Length; i++)
        {
            string textureName = TerrainTextures[i];
            tmpTexture = GameDataHolder.GetResource<Texture2D>(PackType.TexturesDB, textureName);
            if (tmpTexture == null) tmpTexture = default;

            TerrainTexturesArray[i] = tmpTexture;
        }
    }
    private void InitTerrainPool()
    {
        terrainPool = new List<SpoolChunkData>();
        loadedChunks = new List<SpoolChunkData>();

        //FillTerrainSpool(10);

    }

    private void FillTerrainSpool(int ChunkCount) {
        terrainPool = new List<SpoolChunkData>();
        for (int i = 0; i < ChunkCount; i++)
        {
            Vector2Int pos = new Vector2Int(i * CHUNK_SIZE, i * CHUNK_SIZE);
            GameObject spoolChunk = GetNewChunk(pos.x, pos.y, CHUNK_SIZE);
            spoolChunk.SetActive(false);
            terrainPool.Add(new SpoolChunkData(pos, spoolChunk));
        }
    }

    private struct SpoolChunkData
    {
        public Vector2Int pos;
        public GameObject chunk;

        public SpoolChunkData(Vector2Int pos, GameObject chunk)
        {
            this.pos = pos;
            this.chunk = chunk;
        }
    }

    private void CleanupLoadedChunks()
    {
        List<SpoolChunkData> cleanedLoadedChunks = new List<SpoolChunkData>();
        foreach (SpoolChunkData spoolChunkData in loadedChunks)
        {
            if (Vector2Int.Distance(currentChunk, spoolChunkData.pos) <= 3)
            {
                cleanedLoadedChunks.Add(spoolChunkData);
            }
            else
            {
                spoolChunkData.chunk.SetActive(false);
                terrainPool.Add(spoolChunkData);
            }
        }
        loadedChunks = cleanedLoadedChunks;
    }

    private void MoveLoadedChunks(Vector2Int delta)
    {
        Vector3 pos;
        foreach (SpoolChunkData spoolChunkData in loadedChunks)
        {
            pos = spoolChunkData.chunk.transform.position;
            pos.x -= delta.x * CHUNK_SIZE * scale;
            pos.z -= delta.y * CHUNK_SIZE * scale;
            spoolChunkData.chunk.transform.position = pos;
        }

        if (FlatlandTransform != null)
        {
            pos = FlatlandTransform.position;
            pos.x -= delta.x * CHUNK_SIZE * scale;
            pos.z -= delta.y * CHUNK_SIZE * scale;
            FlatlandTransform.position = pos;
        }

    }

    private void InitSectorMarkerCube()
    {
        if (CurrentSectorCube == null)
        {
            CurrentSectorCube = new GameObject
            {
                name = "Current sector marker"
            };

            MeshFilter mf = CurrentSectorCube.AddComponent<MeshFilter>();
            mf.mesh = Instantiate(Resources.GetBuiltinResource<Mesh>("Cube.fbx"));

            MeshRenderer mr = CurrentSectorCube.AddComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            CurrentSectorCube.transform.localScale = new Vector3(scale * CHUNK_SIZE, scale * CHUNK_SIZE, scale * CHUNK_SIZE);
            CurrentSectorCube.transform.parent = World.transform;
            CurrentSectorCube.transform.position = new Vector3(scale * CHUNK_SIZE / 2, 0, scale * CHUNK_SIZE / 2);
        }
    }
    private void LoadPlayerArea()
    {
        currentChunk = GetObjectChunkXY(ObjData.pos);

         if (currentChunk == prevCursorChunk & !IsFirstFrame) return;

        IsFirstFrame = false;
        if (prevCursorChunk == null) prevCursorChunk = currentChunk;
        Vector2Int delta = currentChunk - prevCursorChunk;

        prevCursorChunk = currentChunk;


        Vector3 newPos = cursorObject.transform.position;
        newPos.x = (float) ObjData.pos.x % (CHUNK_SIZE * scale);
        newPos.z = (float) ObjData.pos.z % (CHUNK_SIZE * scale);
        cursorObject.transform.position = newPos;
        ObjData.prevTransformPosition = newPos;

        LoadChunksAreaAsync(currentChunk);
        CleanupLoadedChunks();
        PlaceLoadedChunks();
        PlaceDistantLand();
    }
    /// <summary>
    /// Размещение загруженных блоков террайна в соответсвии с координатами блока наблюдателя
    /// </summary>
    private void PlaceLoadedChunks()
    {
        Vector2Int delta;
        foreach (SpoolChunkData chunk in loadedChunks)
        {
            delta = (currentChunk - chunk.pos);
            chunk.chunk.transform.position = new Vector3(-delta.x * CHUNK_SIZE * scale, chunk.chunk.transform.position.y, -delta.y * CHUNK_SIZE * scale);
        }
    }

    private void PlaceDistantLand()
    {
        if (FlatlandTransform == null) return;
        FlatlandTransform.position = new Vector3(-currentChunk.x * CHUNK_SIZE * scale, FlatlandTransform.position.y, -currentChunk.y * CHUNK_SIZE * scale);

    }
    private void LoadChunksArea(Vector2Int pos,int side=5)
    {
        LoadChunksAreaAsync(pos, side);
    }
  /// <summary>
  /// Загрузить асинхронно группу террайнов размером sideXside блоков в центре pos
  /// </summary>
  /// <param name="pos">Номер блока по x и y</param>
  /// <param name="side">количество блоков по каждой стороне</param>
    private async void LoadChunksAreaAsync(Vector2Int pos, int side=5)
    {
        for (int y = 0; y < side; y++)
        {
            for (int x = 0; x < side; x++)
            {
                Vector2Int newPos = pos + new Vector2Int(x - side/2 -1, y - side/2 - 1);

                if (newPos.y < 0) continue;
                if (newPos.x < 0) continue;
                LoadChunk(newPos);
                await Task.Yield();
            }
        }
    }

    private GameObject GetChunkFromSpool(int startX, int startY, int size)
    {
        Vector2Int pos = new Vector2Int(startX, startY);
        //Debug.Log("Spool size: " + terrainPool.Count);
        if (terrainPool.Count == 0) return GetNewChunk(startX, startY, size);

        
        foreach (SpoolChunkData chunkData in terrainPool)
        {
            if (chunkData.pos == pos)
            {
                //chunkData.chunk.SetActive(true);
                terrainPool.Remove(chunkData);
                return chunkData.chunk;
            }
        }

        int index = terrainPool.Count - 1;
        GameObject chunk = terrainPool[index].chunk;
        terrainPool.RemoveAt(index);

        UpdateChunk(chunk, startX, startY, size);
        //chunk.SetActive(true);
        return chunk;

    }

    private bool IsLoaded(Vector2Int pos)
    {
        foreach (SpoolChunkData chunkData in loadedChunks)
        {
            if (pos == chunkData.pos) return true;
        }
        return false;
    }
    private void LoadChunk(Vector2Int pos)
    {
        if (IsLoaded(pos)) return;
        GameObject chunk = GetChunkFromSpool(pos.x * CHUNK_SIZE, pos.y * CHUNK_SIZE, CHUNK_SIZE);
        Vector2Int OffsetVector = pos - currentChunk;

        //Debug.Log($"Loading {pos} @ {OffsetVector} current {currentChunk}");

        Vector3 chunkPos = new Vector3(OffsetVector.x * CHUNK_SIZE * scale, chunk.transform.position.y, OffsetVector.y * CHUNK_SIZE * scale);

        chunk.transform.position = chunkPos;
        chunk.SetActive(true);
        loadedChunks.Add(new SpoolChunkData(pos, chunk));
    }
    private Vector2Int GetObjectChunkXY(Vector3 pos)
    {
        Vector2Int res = new Vector2Int();

        res.x = (int)System.Math.Floor(pos.x / (scale * CHUNK_SIZE));
        res.y = (int)System.Math.Floor(pos.z / (scale * CHUNK_SIZE));

        return res;

    }
    private Vector2Int GetObjectChunkXY(Vector3d pos)
    {
        Vector2Int res = new Vector2Int();

        //res.x = (int)System.Math.Ceiling(pos.x / (scale * CHUNK_SIZE));
        //res.y = (int)System.Math.Ceiling(pos.z / (scale * CHUNK_SIZE));
        res.x = (int) (pos.x / (scale * CHUNK_SIZE));
        res.y = (int) (pos.z / (scale * CHUNK_SIZE));
        return res;
    }

    public void Initialize()
    {
        IsFirstFrame = true;
        LoadHeader();
        InitMaterials();
        std.OpenSQ(true, false);

        InitCursorObject();

        World = new GameObject
        {
            name = "World"
        };
          LoadFlatland();
        InitTerrainPool();
    }
    public void LoadHeader()
    {
        std = new TERRAIN_DATA(mapName);
        std.OpenHdr();

        //string message = mapName + " Terrain size: " + std.Header.SizeXBPages * TerrainDefs.BOXES_PAGE_SIZE * TerrainDefs.SQUARES_IN_BOX + "x" + std.Header.SizeZBPages * TerrainDefs.BOXES_PAGE_SIZE * TerrainDefs.SQUARES_IN_BOX;
        //Debug.Log(message);

        //Debug.Log(std);
        //std.OpenSQ(false,false);
        //Debug.Log(std.Squares.pager.Get(100, 100));
    }

    private void InitMaterials()
    {
        Stream stream = GameDataHolder.GetResource<Stream>(PackType.rData, "default#terrmtl");
        TerrainMtlCfg terrainMtlCfg = StormFileUtils.ReadStruct<TerrainMtlCfg>(stream);
        
        TerrainTextures = new string[std.Header.nMaterials];
        TerrainTextures1 = new SurfData[std.Header.nMaterials];
        //foreach (int MaterialId in std.Materials.SurType)
        for (int i=0;i<std.Header.nMaterials;i++)
        {
            int SurfaceType = std.Materials.SurType[i];
            SurfaceDesc surfaceDesc = StormFileUtils.ReadStruct<SurfaceDesc>(stream, stream.Position);
            //Debug.Log("Material ID:" + SurfaceType.ToString("X8"));
            //Debug.Log(surfaceDesc);
            TerrainTextures[i] = GameDataHolder.GetNameById(PackType.TexturesDB, surfaceDesc.texture);
            TerrainTextures1[i] = new SurfData
            {
                name = TerrainTextures[i],
                terrainType = (TerrainDefs.GroundType) SurfaceType
            };
        }

        InitTextures();
    }
    [System.Serializable]
    public struct SurfData
    {
        public string name;
        public TerrainDefs.GroundType terrainType;
    }
    private void LoadFlatland()
    {
        if (flatland == null) return;
        int width = std.Squares.pager.SizeX();
        int height = std.Squares.pager.SizeZ();
        GameObject FlatlandCursor = new GameObject
        {
            name = "Flatland cursor"
        };
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = FlatlandCursor.transform;
        plane.name = "Flatland"; plane.transform.position = new Vector3(World.transform.position.x + width * scale / 2, 1, World.transform.position.z + height * scale / 2);
        plane.transform.localScale = new Vector3(-width * scale / 10, 1, -height * scale / 10);
        //plane.transform.parent = transform; 
        MeshRenderer rend = plane.GetComponent<MeshRenderer>(); rend.material = flatland;
        //FlatlandTransform = plane.transform;
        FlatlandTransform = FlatlandCursor.transform;
        FlatlandTransform.parent = World.transform;
    }
    //Flag&SQF_GRMASK



    public struct SectorData
    {
        public float[,] sectorHeightMap;
        public int[,] terrainTypes;
        public int[] terrainTypesEnum;
    }

    private T_SQUARE[] GetDataArray(int startX, int startY, int size)
    {
        string cacheFileName = "Cache/" + $"SectorDataX{startX}Y{startY}Size{size}" + ".xml";
        T_SQUARE[] dataArray = new T_SQUARE[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                dataArray[y * size + x] = std.Squares.pager.Get(startX + x, startY + y);
            }
        }


        //if (!File.Exists(cacheFileName)) StormFileUtils.SaveXML<T_SQUARE[]>(cacheFileName, dataArray);
        //Debug.Log((dataArray.Length,cnt));
        return dataArray;
    }

    private SectorData GetSectorData(Vector2Int coords)
    {
        return GetSectorData(coords.x * CHUNK_SIZE * scale, coords.y * CHUNK_SIZE * scale, CHUNK_SIZE);
    }
    private  SectorData GetSectorData(int startX, int startY, int size)
    {
        T_SQUARE data;
        int groundType;

        float[,] sectorHeightMap = new float[size + 1, size + 1];
        List<int> TTE = new();
        int[,] splatmap = new int[size + 1, size + 1];

        T_SQUARE[] dataArray = GetDataArray(startX, startY, size+1);

        for (int y = 0; y < size + 1; y++)
        {
            for (int x = 0; x < size + 1; x++)
            {
                //data = tmpstd.Squares.pager.Get(startX + x, startY + y);
                data = dataArray[y * (size+1) + x];
                sectorHeightMap[y, x] = NormalizeHeight(data.Height);
                //sectorHeightMap[y, x] = 0;
                //Color heatColor = new Color(sectorHeightMap[y, x], 0, 1 - sectorHeightMap[y, x], 0);
                //heatTexture.SetPixel(x, y, heatColor);
                groundType = data.Flag & TerrainDefs.SQF_GRMASK;

                splatmap[x, y] = groundType;
                if (!TTE.Contains(groundType)) TTE.Add(groundType);
            }
        }
    
        
        SectorData res = new SectorData();
        res.sectorHeightMap = sectorHeightMap;
        res.terrainTypes = splatmap;
        res.terrainTypesEnum = TTE.ToArray();

        
        return res;
    }

    
    private void UpdateChunk (GameObject chunk,Vector2Int coords,int size)
    {
        UpdateChunk(chunk, coords.x, coords.y, size);
    }
    private void UpdateChunk(GameObject chunk, int startX, int startY, int size)
    {

        //SectorData data = await Task.Run(() => GetSectorData(chunk, startX, startY, size));
        SectorData data = GetSectorData(startX, startY, size);
        //SectorData data = await Task.Run(() => GetSectorData(startX, startY, size));
        

        //Debug.Log($"Loading sector [{startX}:{startY}] " + data.terrainTypesEnum.Length);
        int LayersCount = data.terrainTypesEnum.Length;
        float[,,] splatmap2 = new float[size + 1, size + 1, LayersCount];
        TerrainLayer[] terrainLayers = new TerrainLayer[LayersCount];

        for (int i = 0; i < LayersCount; i++)
        {
            TerrainLayer tmpLayer = new TerrainLayer();
            //tmpLayer.name = "Layer " + i + " " + TerrainTextures[i];
            //Debug.Log("Using terrain type: " + i);
            tmpLayer.name = TerrainTextures[data.terrainTypesEnum[i]];
            tmpLayer.tileSize = new Vector2(CHUNK_SIZE * scale / 4, CHUNK_SIZE * scale / 4);
            tmpLayer.diffuseTexture = TerrainTexturesArray[data.terrainTypesEnum[i]];
            terrainLayers[i] = tmpLayer;

            int groundType;
            for (int y = 0; y < size + 1; y++)
            {
                for (int x = 0; x < size + 1; x++)
                {
                    //groundType = splatmap[y, x];
                    groundType = data.terrainTypes[y, x];
                    splatmap2[x, y, i] = 0;
                    if (groundType == data.terrainTypesEnum[i]) splatmap2[x, y, i] = 1;
                }
            }
        }
        TerrainData terrainData = chunk.GetComponent<Terrain>().terrainData;
        terrainData.name = "TDATA " + startX + " " + startY;
        terrainData.SetHeights(0, 0, data.sectorHeightMap);


        //TerrainLayer[] terrainLayers = terrainData.terrainLayers;
        //terrainLayers[0].diffuseTexture = heatTexture;

        terrainData.terrainLayers = terrainLayers;
        terrainData.SetAlphamaps(0, 0, splatmap2);
        chunk.name = "Terrain " + startX + " " + startY;
    }

    private GameObject GetNewChunk(int startX, int startY, int size)
    {
        TerrainData terrainData = new TerrainData();
        terrainData.baseMapResolution = CHUNK_SIZE;
        terrainData.heightmapResolution = size + 1;
        terrainData.alphamapResolution = size + 1;
        terrainData.SetDetailResolution(1024, CHUNK_SIZE);
        terrainData.name = "TDATA " + startX + " " + startY;
        terrainData.size = new Vector3(size * scale, size * scale, size * scale);

        GameObject terrain = Terrain.CreateTerrainGameObject(terrainData);
        //terrain.SetActive(false);
        terrain.name = "Terrain " + startX + " " + startY;

        terrain.transform.position = new Vector3(0, (0 - terrainData.size.y / 2), 0); //set terrain on default water level
        //terrain.transform.parent = transform;
        terrain.transform.parent = World.transform;

        UpdateChunk(terrain, startX, startY, size);
        //terrain.SetActive(true);
        return terrain;
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
