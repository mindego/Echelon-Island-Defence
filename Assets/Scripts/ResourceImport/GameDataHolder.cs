using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.InteropServices;

public class GameDataHolder
{
    public static GameDataHolder instance;
    private IGameDataProvider gameDataProvider;

    public GameDataHolder()
    {
        Debug.Log("Me new holder!");
        gameDataProvider = new GameDataProviderAssets();
    }

    ~GameDataHolder()
    {
        Debug.Log("Data holder unloaded");
    }

    public static ResourcePack GetMeshDB()
    {
        return GetInstance().gameDataProvider.GetPack(PackType.MeshDB);
    }

    public static ResourcePack GetTexturesDB()
    {
        return GetInstance().gameDataProvider.GetPack(PackType.TexturesDB);
    }

    public static GameDataHolder GetInstance() 
    {
        if (instance == null)
        {
            instance = new GameDataHolder();
        }
        return instance;
    }

    public static string[] ListContent(PackType packId)
    {
        return GetInstance().gameDataProvider.ListContent(packId);
    }

    public static Dictionary<string,uint> GetContentDictionary(PackType packId)
    {
        return GetInstance().gameDataProvider.GetContentDictionary(packId);
    }
    

    public static T GetResource<T>(PackType packId, string name)
    {
        return GetInstance().gameDataProvider.GetResource<T>(packId, name);
    }

    public static string GetNameById(PackType packId,uint id)
    {
        return GetInstance().gameDataProvider.GetNameById(packId, id);
    }
    public static T GetResource<T>(PackType packId,uint id,string name = null)
    {
        return GetInstance().gameDataProvider.GetResource<T>(packId, id, name);
    }
    public static ResourcePack GetPack(PackType packId)
    {
        return GetInstance().gameDataProvider.GetPack(packId);
    }
}
