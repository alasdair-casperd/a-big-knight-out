using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "TilePrefabManager", menuName = "Scriptable Objects/TilePrefabManager")]
public class TilePrefabManager : ScriptableObject
{
    public GameObject[] prefabs;

    /// <summary>
    /// Returns the game object associated with a given tile type.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public GameObject GetPrefab(TileType tile)
    {
        return prefabs[tile.ID];
    }
}