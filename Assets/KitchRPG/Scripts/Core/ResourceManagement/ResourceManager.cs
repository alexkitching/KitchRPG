using System;
using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public enum AssetType
{
    Prefab,
    TextAsset,
    Texture2D,
    Sprite,
    AudioClip
}

public static class ResourceManager
{
    private static ResourceMap resourceMap;

    private static Dictionary<AssetType, AssetMap> assetMapLookup;

    public static void Start(ResourceMap a_resourceMap)
    {
        resourceMap = a_resourceMap;

        int resourceMapLen = resourceMap.map.Length;
        assetMapLookup = new Dictionary<AssetType, AssetMap>(resourceMapLen);
        for (int i = 0; i < resourceMapLen; ++i)
        {
            assetMapLookup.Add(resourceMap.map[i].assetType, resourceMap.map[i]);
        }
    }

    public static T LoadResource<T>(string a_path, AssetType a_type) where T : UnityEngine.Object
    {
        AssetMap map = null;
        if(assetMapLookup.TryGetValue(a_type, out map) == false) // No Asset Map Found
        {
            Debug.LogError("Resource Manager was unable to find an Asset Map for Asset Type: " + a_type);
            return null;
        }

        int bundleNameEndIndex = a_path.IndexOf("/", 0, StringComparison.Ordinal);
        string bundleName = a_path.Substring(0, bundleNameEndIndex);

        PathMap pathMap = null;

        if(map.PathMap.TryGetValue(bundleName.GetHashCode(), out pathMap) == false) // No Asset Bundle Found
        {
            Debug.LogError("ResourceManager was unable to find an Asset Bundle of Name: " + bundleName);
            return null;
        }

        string finalPath;
        pathMap.TryGetValue(a_path.GetHashCode(), out finalPath);

        return Resources.Load<T>(a_path);
    }

    //public static T LoadResource<T>(string a_bundleName, string a_assetName, AssetType a_type) where T : UnityEngine.Object
    //{
    //    AssetMap map = null;
    //    if(assetMapLookup.TryGetValue(a_type, out map) == false) // No Asset Map Found
    //    {
    //        Debug.LogError("Resource Manager was unable to find an Asset Map for Asset Type: " + a_type);
    //        return null;
    //    }
    //
    //    PathMap pathMap = null;
    //
    //    if(map.PathMap.TryGetValue(a_bundleName.GetHashCode(), out pathMap) == false) // No Asset Bundle Found
    //    {
    //        Debug.LogError("ResourceManager was unable to find an Asset Bundle of Name: " + bundleName);
    //        return null;
    //    }
    //
    //    string finalPath;
    //    pathMap.TryGetValue(a_path.GetHashCode(), out finalPath);
    //
    //    return Resources.Load<T>(a_path);
    //}
}

public struct ResourceMapping
{
    private string path;
    private int hash;
}
