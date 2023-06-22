using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;

public class GameDataCache
{
    public static GameDataCache instance;
    private Dictionary<string, object> cache=new Dictionary<string, object>();

    public static GameDataCache getInstance()
    {
        if (instance == null) instance = new GameDataCache();
        return instance;
    }

    public static void AddCacheStatic(string key,object value)
    {
        GameDataCache instance = getInstance();
        instance.AddCache(key, value);
    }
    
    public void AddCache(string key, object value)
    {
        Debug.Log("cache item added as " + key);
        cache.Add(key, value);
    }

    public static T GetCacheStatic<T>(string key)
    {
        GameDataCache instance = getInstance();
        return instance.GetCache<T>(key);
    }

    public T GetCache<T>(string key)
    {
        object value;
        bool isPresent = cache.TryGetValue(key, out value);
        if (!isPresent) return default;

        Debug.Log("Cache hit!");
        return (T)value;
    }

    public static bool isPresentStatic(string key)
    {
        GameDataCache instance = getInstance();
        return instance.isPresent(key);
    }
    public bool isPresent(string key)
    {
        return cache.ContainsKey(key);
    }

    
}
