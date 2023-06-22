using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataStreamer
{
    public static GameDataStreamer instance;
    private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
    private Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();
    private Dictionary<string, Material> materials = new Dictionary<string, Material>();
    private Dictionary<string, ImportMeshEditor.meshData> meshData = new Dictionary<string, ImportMeshEditor.meshData>();


    public static GameDataStreamer getInstance()
    {
        if (GameDataStreamer.instance == null) GameDataStreamer.instance = new GameDataStreamer();
        return GameDataStreamer.instance;
    }

    public GameDataStreamer()
    {
        if (instance != null) return;

        instance = this;
    }

    public static T GetResource<T>(string resourceName)
    {
        return getInstance().GetResourceFromDB<T>(resourceName);
    }

    public static void AddResource<T>(string resourceName,T myObject)
    {
        getInstance().AddResourceToDB(resourceName, myObject);
    }
    private void AddResourceToDB<T>(string resourceName, T myObject)
    {
        if (resourceName == null) return;
        packId packId = GetPackId<T>();
        switch (packId)
        {
            case packId.Textures:
                if (textures.ContainsKey(resourceName)) break;
                textures.Add(resourceName, (Texture2D) (object) myObject);
                break;
            case packId.Meshes:
                if (meshes.ContainsKey(resourceName)) break;
                meshes.Add(resourceName, (Mesh) (object) myObject);
                break;
            case packId.Materials:
                if (materials.ContainsKey(resourceName)) break;
                materials.Add(resourceName, (Material) (object) myObject);
                break;
            case packId.meshData:
                if (meshData.ContainsKey(resourceName)) break;
                meshData.Add(resourceName, (ImportMeshEditor.meshData) (object)myObject);
                break;
        }

    }
    private T GetResourceFromDB<T>(string resourceName) {
        packId packId = GetPackId<T>();
        switch (packId)
        {
            case packId.Textures:
                return (T) (object) textures.GetValueOrDefault(resourceName);
            case packId.Meshes:
                return (T) (object) meshes.GetValueOrDefault(resourceName);
            case packId.Materials:
                return (T)(object) materials.GetValueOrDefault(resourceName);
            case packId.meshData:
                return (T)(object)meshData.GetValueOrDefault(resourceName);
        }
        return default;
    }

    private packId GetPackId<T>()
    {
        if (typeof(T) == typeof(Texture2D)) return packId.Textures;
        if (typeof(T) == typeof(Mesh)) return packId.Meshes;
        if (typeof(T) == typeof(Material)) return packId.Materials;
        if (typeof(T) == typeof(ImportMeshEditor.meshData)) return packId.meshData;

        return default;
    }

    public enum packId
    {
        Textures,
        Meshes,
        Materials,
        meshData

    }
}
