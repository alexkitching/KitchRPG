using System.Collections;
using System.Collections.Generic;
using System.IO;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

[System.Serializable]
public class PathMap : SerializableDictionaryBase<int, string>
{
}

[System.Serializable]
public class ResourceBundleMap : SerializableDictionaryBase<int, PathMap>
{
}

[System.Serializable]
public class AssetMap
{
    public AssetType assetType;
    public string Extension;
    public ResourceBundleMap PathMap;
}

[CreateAssetMenu(fileName = "ResourceMap", menuName = "ResourceMap")]
public class ResourceMap : ScriptableObject
{
    public AssetMap[] map;

    [ContextMenu("Map Resources")]
    public void MapResources()
    {
        for (int i = 0; i < map.Length; ++i)
        {
            map[i].PathMap.Clear();
        }
        ResourceMapper.TryMapDirectory("Assets/Resources/", map[0]);
    }
}

public static class ResourceMapper
{
    private static LinkedList<string> referenceList = new LinkedList<string>();

    public static bool TryMapDirectory(string a_RootDir, AssetMap a_map)
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
            MapResBundleFolder(childDirs[i], a_map);
        }
        
        return true;
        //FileInfo[] info = dir.GetFiles("")
    }

    private static void MapResBundleFolder(DirectoryInfo a_bundleDir, AssetMap a_map)
    {
        string dirName = a_bundleDir.Name;
        int dirHash = dirName.GetHashCode();

        PathMap pathMap = null;
        if(a_map.PathMap.TryGetValue(dirHash, out pathMap) == false) // No Path Map Exists
        {
            pathMap = new PathMap();
            a_map.PathMap.Add(dirHash, pathMap);
        }

        string extension = a_map.Extension;

        RecursivelyMapBundleDir(a_bundleDir, pathMap, extension);

        Debug.Log("Map Directory Complete!");
    }

    private static void RecursivelyMapBundleDir(DirectoryInfo a_directory, PathMap a_pathMap, string extension)
    {
        MapResBundleFiles(a_directory, a_pathMap, extension);
        DirectoryInfo[] subDirectoryInfos = a_directory.GetDirectories();

        for (int i = 0; i < subDirectoryInfos.Length; ++i) // Map Children First
        {
            RecursivelyMapBundleDir(subDirectoryInfos[i], a_pathMap, extension);
            referenceList.RemoveLast();
        }
    }

    private static void MapResBundleFiles(DirectoryInfo a_dir, PathMap a_map, string a_extension)
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
            a_map.Add(pathHash, fullPath);
            // Get Full Path from ReferenceStack
        }
    }

}
