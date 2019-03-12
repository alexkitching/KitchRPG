using System.Collections;
using System.Collections.Generic;
using System.IO;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class PathMap : SerializableDictionaryBase<int, string> // Assets
{
}

[System.Serializable]
public class BundleMap : SerializableDictionaryBase<AssetType, PathMap>// Assets by type
{
    public BundleMap()
    {
        int count = (int) AssetType.COUNT;
        for (int i = 0; i < count; ++i)
        {
            AssetType type = (AssetType) i;
            this.Add(type, new PathMap());
        }
    }
}

[System.Serializable]
public class ResourceBundleDictionary : SerializableDictionaryBase<BundleName, BundleMap> // Collection of Assets
{
    private BundleName name;
    public BundleMap TryGetBundleMap(string a_bundleName)
    {
        name.hash = a_bundleName.GetHashCode();
        return this[name];
    }

    public BundleMap TryGetBundleMap(int a_bundleNameHash)
    {
        name.hash = a_bundleNameHash;
        return this[name];
    }
}

[System.Serializable]
public class BundleName
{
    [SerializeField]
    private string name;
    public int hash;

    public BundleName()
    {
        hash = name.GetHashCode();
    }

    public BundleName(string a_name)
    {
        name = a_name;
        hash = name.GetHashCode();
    }

    public override int GetHashCode()
    {
        return hash;
    }
}


[CreateAssetMenu(fileName = "ResourceMap", menuName = "ResourceMap")]
public class ResourceMap : ScriptableObject
{
    [SerializeField] 
    public ResourceBundleDictionary bundles = new ResourceBundleDictionary();

    [SerializeField] 
    public PathMap[] types = new PathMap[(int)AssetType.COUNT];

    [ContextMenu("Map Resources")]
    public void MapResources()
    {
        bundles = new ResourceBundleDictionary();
        for (int i = 0; i < types.Length; ++i)
        {
            types[i].Clear();
        }
        
        ResourceMapper.Run(bundles, ref types);
    }
}

public static class ResourceMapper
{
    private static LinkedList<string> referenceList = new LinkedList<string>();

    private class ResourceBundleSpec
    {
        public ResourceBundleSpec(string a_name, string a_path)
        {
            name = a_name;
            path = a_path;
        }
        public string name;
        public string path;
    }

    private static ResourceBundleSpec[] bundles =
    {
        new ResourceBundleSpec("UI", "UI")
    };

    private static Dictionary<AssetType, string> AssetTypeExtensionLookup = new Dictionary<AssetType, string>()
    {
        {AssetType.Prefab, ".prefab"}
    };


    private static ResourceBundleDictionary currentBundleMap;
    private static PathMap[] currentTypeMaps;
    private static string ResourcesRoot = "Assets/Resources/";

    public static void Run(ResourceBundleDictionary a_bundleMaps, ref PathMap[] a_typeMap)
    {
        currentBundleMap = a_bundleMaps;
        currentTypeMaps = a_typeMap;

        TryMapResourceBundles();
    }

    private static bool TryMapResourceBundles()
    {
        DirectoryInfo dir = new DirectoryInfo(ResourcesRoot);

        if (dir.Exists == false)
        {
            Debug.LogError("Directory Map Failed: Resources Root Directory does not exist!");
            return false;
        }

        referenceList.Clear();

        
        for (int i = 0; i < bundles.Length; ++i)
        {
            DirectoryInfo currentBundleDir = new DirectoryInfo(ResourcesRoot + bundles[i].path);

            if (dir.Exists == false)
            {
                Debug.LogError("Bundle Map Failed: " + bundles[i].path + " Directory does not exist!");
                continue;
            }
            MapResBundleFolder(currentBundleDir);
        }

        return true;
    }

    public static bool TryMapDirectory(string a_RootDir)
    {
        DirectoryInfo dir = new DirectoryInfo(a_RootDir);

        if (dir.Exists == false)
        {
            Debug.LogError("Directory Map Failed: Directory does not exist!");
            return false;
        }

        referenceList.Clear();

        DirectoryInfo[] childDirs = dir.GetDirectories();

        for (int i = 0; i < childDirs.Length; ++i)
        {
            MapResBundleFolder(childDirs[i]);
        }
        
        return true;
    }

    private static void MapResBundleFolder(DirectoryInfo a_bundleDir)
    {
        string dirName = a_bundleDir.Name;

        BundleMap bundleMap = null;
        BundleName bundleName = new BundleName(dirName);
        if(currentBundleMap.TryGetValue(bundleName, out bundleMap) == false) // No Path Map Exists
        {
            bundleMap = new BundleMap();
            currentBundleMap.Add(bundleName, bundleMap);
        }

        for (int i = 0; i < (int) bundleMap.Count; ++i)
        {
            AssetType type = (AssetType)i;
            string extension = AssetTypeExtensionLookup[type];
            RecursivelyMapBundleDir(a_bundleDir, bundleMap[type], currentTypeMaps[i], extension);
        }

        Debug.Log("Map Directory Complete!");
    }

    private static void MapResBundleFolder(string a_bundleName, string a_bundlePath)
    {
        string dirName = a_bundleName;

        BundleMap bundleMap = null;
        BundleName bundleName = new BundleName(dirName);
        if(currentBundleMap.TryGetValue(bundleName, out bundleMap) == false) // No Path Map Exists
        {
            bundleMap = new BundleMap();
        }

        DirectoryInfo bundleDir = new DirectoryInfo(a_bundlePath);

        for (int i = 0; i < (int) bundleMap.Count; ++i)
        {
            AssetType type = (AssetType)i;
            string extension = AssetTypeExtensionLookup[type];
            RecursivelyMapBundleDir(bundleDir, bundleMap[type], currentTypeMaps[i], extension);
        }

        Debug.Log("Map Directory Complete!");
    }

    private static void RecursivelyMapBundleDir(DirectoryInfo a_directory, PathMap a_bundlePathMap, PathMap a_globalPathMap, string extension)
    {
        MapResBundleFiles(a_directory, a_bundlePathMap, a_globalPathMap, extension);
        DirectoryInfo[] subDirectoryInfos = a_directory.GetDirectories();

        for (int i = 0; i < subDirectoryInfos.Length; ++i) // Map Children First
        {
            RecursivelyMapBundleDir(subDirectoryInfos[i], a_bundlePathMap, a_globalPathMap,extension);
            referenceList.RemoveLast();
        }
    }

    private static void MapResBundleFiles(DirectoryInfo a_dir, PathMap a_bundlePathMap, PathMap a_globalPathMap, string a_extension)
    {
        referenceList.AddLast(a_dir.Name);
        FileInfo[] info = a_dir.GetFiles("*" + a_extension);

        LinkedListNode<string> node = null;
        for (int i = 0; i < info.Length; ++i)
        {
            node = referenceList.First;
            
            string fullName = info[i].Name;
            
            int extensionIndex = fullName.IndexOf(".", System.StringComparison.Ordinal);
            string name = fullName.Substring(0, extensionIndex);
            string extension = fullName.Substring(extensionIndex, fullName.Length - extensionIndex);

            string fullPath = "";
            while (node != referenceList.Last)
            {
                fullPath += node.Value;
                fullPath += "/";
                node = node.Next;
            }
            fullPath += node.Value;
            fullPath += "/";

            fullPath += name;
            int pathHash = fullPath.GetHashCode();
            fullPath += extension;
            a_bundlePathMap.Add(pathHash, fullPath);
            a_globalPathMap.Add(pathHash, fullPath);
        }
    }
}
