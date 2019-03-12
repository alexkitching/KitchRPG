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
    AudioClip,
    COUNT
}

public static class ResourceManager
{
    private static ResourceMap resourceMap;

    public static void Start(ResourceMap a_resourceMap)
    {
        if(ValidateMap(a_resourceMap))
            resourceMap = a_resourceMap;
    }

    private static bool ValidateMap(ResourceMap a_resourceMap)
    {
        int assetTypeCount = (int) AssetType.COUNT;
        if (a_resourceMap.types.Length < assetTypeCount)
        {
            Debug.LogError("ResourceMap does not contain maps for all asset types!" +
                      " Expected: " + assetTypeCount +
                      " Recieved: " + a_resourceMap.types.Length);
            return false;
        }

        Stack<AssetType> nullTypes = new Stack<AssetType>();
        for (int i = 0; i < assetTypeCount; ++i)
        {
            if(a_resourceMap.types[i] == null) // No Asset Map Found
            {
                nullTypes.Push((AssetType)i);
            }
        }

        if (nullTypes.Count > 0)
        {
            string errorMsg = "Resource Manager was unable to find an Path Map for the following Asset Type/s: \n";

            while (nullTypes.Count > 0)
            {
                errorMsg += nullTypes.Pop() + ", \n";
            }

            Debug.LogError(errorMsg);
            return false;
        }

        return true;
    }

    public static T LoadResource<T>(string a_path, AssetType a_type) where T : UnityEngine.Object
    {
#if DEBUG
        if(resourceMap == null)
            DebugInstantiateManager();
#endif


        int bundleNameEndIndex = a_path.IndexOf("/", 0, StringComparison.Ordinal);
        string bundleName = a_path.Substring(0, bundleNameEndIndex);
        // Try to map to bundle

        BundleMap bundleMap = null;
        string finalPath;
        PathMap pathMap = null;

        bool found = false;
        if (resourceMap.bundles.TryGetBundleMap(bundleName, out bundleMap)) // Bundle Found
        {
            if (bundleMap.TryGetValue(a_type, out pathMap))
            {
                found = pathMap.TryGetValue(a_path.GetHashCode(), out finalPath);
            }
        }

        if (!found) // Search Global Asset Map
        {
            pathMap = resourceMap.types[(int) a_type];
            int pathHash = a_path.GetHashCode();
            found = pathMap.TryGetValue(pathHash, out finalPath);
        }

        if (!found)
        {
            Debug.LogError("ResourceManager was unable to find an Asset with path " + a_path + " of type " + a_type);
            return null;
        }

        return Resources.Load<T>(a_path);
    }

#if DEBUG
    private static void DebugInstantiateManager()
    {
        ResourceMap map = (ResourceMap)Resources.Load("ResourceMap");
        if (map != null)
        {
            Start(map);
        }
    }
#endif
}