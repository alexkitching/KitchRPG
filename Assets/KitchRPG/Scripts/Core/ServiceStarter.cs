using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceStarter : MonoBehaviour
{
    [SerializeField]
    private GameSceneLookup SceneLookup;

    [SerializeField] 
    private GamingEnvironment gamingEnvironment;

    void Awake()
    {
        SceneManager.Start(gamingEnvironment, SceneLookup, GameSceneEnum.Bootstrap);


        SceneManager.LoadSceneAsync(GameSceneEnum.Preload, LoadSceneMode.Single);
    }
}

