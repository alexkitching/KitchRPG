using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Preload : MonoBehaviour
{
    [SerializeField]
    private ResourceMap resourceMap;

    void Awake()
    {
        ResourceManager.Start(resourceMap);

        //GameObject go = ResourceManager.LoadResource<GameObject>("UI/TestPrefab", AssetType.Prefab);
        //GameObject go = Resources.Load<GameObject>("TestFail");
        //Instantiate(go);
    }
}
