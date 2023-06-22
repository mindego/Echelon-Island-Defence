using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class GameDataProviderAssets : IGameDataProvider
{
    public ResourcePack MeshDB;
    public ResourcePack TexturesDB;
    public ResourcePack MaterialsDB;
    public ResourcePack MEODB;
    public ResourcePack FPODB;
    public ResourcePack gData;
    public ResourcePack rData;
    public ResourcePack ParticlesDB;
    public ResourcePack Voice0DB; //Рита
    public ResourcePack Voice1DB;
    public ResourcePack Voice2DB;

    private Dictionary<PackType, ResourcePack> packs = new Dictionary<PackType, ResourcePack>();
    private GameDataProviderAssetsCache StormCache = new GameDataProviderAssetsCache();

    private Dictionary<string, ImportMeshEditor.meshData> meshInfo;
    private ResourceReplacer resourceReplacer;

    public GameDataProviderAssets()
    {
        Debug.Log("Assets data storage loaded");
        Initialize();
    }
    ~GameDataProviderAssets()
    {
        Debug.Log("Assets data storage unloaded");
    }

    private void Initialize()
    {
        packs.Add(PackType.MeshDB, new ResourcePack("Data/Graphics/mesh.dat"));
        packs.Add(PackType.TexturesDB, new ResourcePack("Data/Graphics/textures.dat"));
        packs.Add(PackType.MaterialsDB, new ResourceStorage<Material>("Data/Graphics/materials.dat"));
        packs.Add(PackType.MEODB, new ResourcePack("Data/objects.dat"));
        packs.Add(PackType.FPODB, new ResourcePack("Data/objects2.dat"));
        packs.Add(PackType.gData, new ResourcePack("Data/gdata.dat"));
        packs.Add(PackType.rData, new ResourcePack("Data/Graphics/rdata.dat"));
        packs.Add(PackType.ParticlesDB, new ResourcePack("Data/Graphics/particles.dat"));
        packs.Add(PackType.Voice0DB, new ResourcePack("Data/Voice0.sfx"));
        packs.Add(PackType.Voice1DB, new ResourcePack("Data/Voice1.sfx"));
        packs.Add(PackType.Voice2DB, new ResourcePack("Data/Voice2.sfx"));
    }
    //private void Initialize()
    //{
    //    MeshDB = new ResourcePack("Data/Graphics/mesh.dat");
    //    TexturesDB = new ResourcePack("Data/Graphics/textures.dat");
    //    //MaterialsDB = new ResourcePack("Data/Graphics/materials.dat");
    //    MaterialsDB = new ResourceStorage<Material>("Data/Graphics/materials.dat");
    //    MEODB = new ResourcePack("Data/objects.dat");
    //    FPODB = new ResourcePack("Data/objects2.dat");
    //    gData = new ResourcePack("Data/gdata.dat");
    //    rData = new ResourcePack("Data/Graphics/rdata.dat");
    //    ParticlesDB = new ResourcePack("Data/Graphics/particles.dat");
    //    Voice0DB = new ResourcePack("Data/Voice0.sfx");
    //    Voice1DB = new ResourcePack("Data/Voice1.sfx");
    //    Voice2DB = new ResourcePack("Data/Voice1.sfx");

    //    //resourceReplacer = StormFileUtils.LoadXML<ResourceReplacer>("Assets/Database/Replacer.xml");
    //    //LoadMeshInfo();
    //}

    private void LoadMeshInfo()
    {
        meshInfo = new Dictionary<string, ImportMeshEditor.meshData>();
      
        XmlSerializer serializer = new XmlSerializer(typeof(ImportMeshEditor.meshData[]));
        FileStream ms = File.Open("Assets/Database/MeshData.xml", FileMode.Open);

        ImportMeshEditor.meshData[] tmpmeshInfo = (ImportMeshEditor.meshData[])serializer.Deserialize(ms);
        for (int i = 0; i < tmpmeshInfo.Length; i++)
        {
            ImportMeshEditor.meshData meshData = tmpmeshInfo[i];

            //foreach (ImportMeshEditor.meshData meshData in tmpmeshInfo) {
            //Debug.Log(meshData.meshName);
            if (meshData.meshName == null) //TODO! Выяснить, почему пустой meshData
            {
                //Debug.LogError("Empty name " + i + " of " + tmpmeshInfo.Length) ;
                //Debug.Log("Prev: "+ tmpmeshInfo[i - 1]);
                continue;

            }
            else
            {
                meshInfo.Add(meshData.meshName, meshData);
            }
        }
    }

    public string GetNameById(PackType packId, uint id)
    {
        ResourcePack pack = GetPack(packId);

        if (pack == null) return null;
        foreach (DbIndex item in pack.RAT.index)
        {
            if (item.object_id == id) return pack.GetNameById(id);
        }

        return null;
    }

    public ResourcePack GetPack(PackType packId) //Так делать - идиотизм! Надо переделать
    {
        return packs[packId];
        //ResourcePack pack;
        //switch (packId)
        //{
        //    case PackType.MeshDB:
        //        pack = MeshDB;
        //        break;
        //    case PackType.TexturesDB:
        //        pack =TexturesDB;
        //        break;
        //    case PackType.MaterialsDB:
        //        pack = MaterialsDB;
        //        break;
        //    case PackType.FPODB:
        //        pack = FPODB;
        //        break;
        //    case PackType.MEODB:
        //        pack = MEODB;
        //        break;
        //    case PackType.gData:
        //        pack = gData;
        //        break;
        //    case PackType.rData:
        //        pack = rData;
        //        break;
        //    case PackType.ParticlesDB:
        //        pack = ParticlesDB;
        //        break;
        //    case PackType.Voice0DB:
        //        pack = Voice0DB;
        //        break;
        //    case PackType.Voice1DB:
        //        pack = Voice1DB;
        //        break;
        //    case PackType.Voice2DB:
        //        pack = Voice2DB;
        //        break;
        //    default:
        //        Debug.Log("Pack not found " + packId);
        //        return null;
        //}
        //return pack;
    }


    public T GetResource<T>(PackType packId, string name)
    {
        //if (packId == PackType.TexturesDB) {
        //    foreach (ResourceReplacer.ReplaceKeyPair rkp in resourceReplacer.ReplaceKeyPairValues)
        //    {
        //        if (rkp.original == name) return GetResource<T>(packId, rkp.replaced);
        //    }
        //}
        ResourcePack pack;
        pack = GetPack(packId);

        if (pack == null) return default(T);
        uint id = pack.GetIdByName(name);
        string cacheKey = packId.ToString() + id.ToString("X8") + name;
        //Debug.Log(cacheKey + " " + name);
        if (StormCache.cacheData.ContainsKey(cacheKey))
        {
            return (T)(object)StormCache.cacheData[cacheKey].content;
        }

        return GetResource<T>(packId, id, name);
    }

    public T GetResource<T>(PackType packId, uint id, string name = null)
    {
        if (id == 0xFFFFFFFF) return default; //перенести в соответствующий импорт?
        ResourcePack pack;
        object data;
        Stream ms;

        pack = GetPack(packId);
        ms = pack.GetStreamById(id);
        if (name == null)
        {
            name = pack.GetNameById(id);
        }

        data = default;
        if (typeof(T) == typeof(Texture2D))
        {
            //data = (T)(object)TextureImport.GetTexture(ms, (int)id);
            data = TextureImport.GetTexture(ms, name);
            //GameDataCache.AddCacheStatic(cacheKey, data);
        };

        if (typeof(T) == typeof(FpoGraphData))
        {
            data = FpoImport.GetMeshMeta(ms);
            //GameDataCache.AddCacheStatic(cacheKey, data);
        }

        if (typeof(T) == typeof(Material))
        {
            // data = (T) (object) MaterialImport.GetMaterial(id);
            data = MaterialImport.GetMaterial(ms, name);
            // data = meshInfo[name];
        }

        if (typeof(T) == typeof(Stream))
        {
            data = ms;
        }

        if (typeof(T) == typeof(AudioClip))
        {
            data = SoundImport.GetAudioClip(ms, name);
        }

        switch (packId)
        {
            case PackType.MeshDB:
                if (typeof(T) == typeof(GameObject))
                {
                    data = StormMeshImport.GetModel(ms, name);
                }
                if (typeof(T) == typeof(Mesh))
                {
                    data = StormMeshImport.GetMesh(ms, name);
                    //GameDataCache.AddCacheStatic(cacheKey, data);
                }
                if (typeof(T) == typeof(StormMesh))
                {
                    data = StormMeshImport.LoadStormMesh(ms, name);
                    //GameDataCache.AddCacheStatic(cacheKey, data);
                }
                break;
            case PackType.FPODB:
                if (typeof(T) == typeof(GameObject))
                {
                    //data = (T)(object)FpoImport.GetFPO(ms, name);
                    data = FpoImport.LoadFPO(ms, name);
                }
                if (typeof(T) == typeof(FpoImport.FPONode))
                {
                    data = FpoImport.GetFPOSchema(ms);
                }
                break;
            case PackType.MEODB:

                break;

        }

        //instance.cache.AddCache(cacheKey,data);
        GameDataProviderAssetsCacheItem cacheItem = new GameDataProviderAssetsCacheItem()
        {
            type = typeof(T),
            content = data
        };
        //if (typeof(T) != typeof(Stream)) StormCache.AddToCache(packId.ToString() + id.ToString("X8"),cacheItem);
        if (typeof(T) != typeof(Stream)) StormCache.AddToCache(packId.ToString() + id.ToString("X8") + name, cacheItem);
        return (T) data;
    }

    public string[] ListContent(PackType packId)
    {
        ResourcePack rp = GetPack(packId);
        string[] list = rp.names;
        return list;
    }

    public Dictionary<string, uint> GetContentDictionary(PackType packId)
    {
        Dictionary<string, uint> res = new Dictionary<string, uint>();
        ResourcePack rp = GetPack(packId);
        uint objId;
        foreach (DbIndex item in rp.RAT.index)
        {
            objId = item.object_id;
            res.Add(rp.GetNameById(objId), objId);
        }

        return res;
    }
}

public class GameDataProviderAssetsCache
{
    public Dictionary<string, GameDataProviderAssetsCacheItem> cacheData = new Dictionary<string, GameDataProviderAssetsCacheItem>();

    public void ClearCache()
    {
        cacheData = new Dictionary<string, GameDataProviderAssetsCacheItem>();
    }

    public void AddToCache(string key, GameDataProviderAssetsCacheItem data)
    {
        if (cacheData.ContainsKey(key)) return;

        cacheData.Add(key, data);
    }
}

public struct GameDataProviderAssetsCacheItem
{
    public Type type;
    public object content;
}
