using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class Preload : MonoBehaviour
{
    void Awake()
    {
        // Setup ResourceManager
        ResourceMap map = (ResourceMap)Resources.Load("ResourceMap");
        if (map != null)
        {
            ResourceManager.Start(map);
        }
        else
        {
            Debug.LogError("ResourceManager Setup Failed! Unable to find ResourceMap in Resources!");
            return;
        }
        

        SceneManager.LoadSceneAsync(GameSceneEnum.Game, LoadSceneMode.Single);
    }
}
