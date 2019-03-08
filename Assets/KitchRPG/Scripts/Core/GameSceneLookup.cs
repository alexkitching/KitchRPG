using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditor;
using UnityEngine;

public enum GameSceneEnum
{
    Bootstrap,
    Preload,
    Game
}

[System.Serializable]
public struct GameSceneMapping
{
    public GameSceneEnum Enum;
    public int index;
}

[System.Serializable]
public class Lookup : SerializableDictionaryBase<GameSceneEnum, int>{};

[CreateAssetMenu(fileName = "SceneLookup")]
public class GameSceneLookup : ScriptableObject
{
    public Lookup lookup;

    public int GetSceneIndex(GameSceneEnum a_scene)
    {
        int index = -1;

        lookup.TryGetValue(a_scene, out index);

        return index;
    }
}