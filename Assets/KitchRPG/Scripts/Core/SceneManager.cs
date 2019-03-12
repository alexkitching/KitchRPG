using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneManager
{
    private static GamingEnvironment enviroment;

    private static GameSceneLookup lookup;

    private static GameSceneEnum currentScene;

    private static IEnumerator loadCoroutine;

    public static void Start(GamingEnvironment a_environment, GameSceneLookup a_lookup, GameSceneEnum a_startScene)
    {
        enviroment = a_environment;
        currentScene = a_startScene;
        lookup = a_lookup;
    }

    public static void LoadScene(GameSceneEnum a_scene, LoadSceneMode a_mode)
    {
        int index = lookup.GetSceneIndex(a_scene);

        UnityEngine.SceneManagement.SceneManager.LoadScene(index, a_mode);

        currentScene = a_scene;
    }

    public static void LoadSceneAsync(GameSceneEnum a_scene, LoadSceneMode a_mode)
    {
        enviroment.StartCoroutine(LoadCoroutine(a_scene));
    }

    private static IEnumerator LoadCoroutine(GameSceneEnum a_scene)
    {
        int index = lookup.GetSceneIndex(a_scene);

        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);

        currentScene = a_scene;
    }
}


