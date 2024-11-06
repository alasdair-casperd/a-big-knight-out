using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "TilePrefabManager", menuName = "Scriptable Objects/TilePrefabManager")]
public class TilePrefabManager : ScriptableObject
{
    [SerializeField]
    List<TilePrefabPair> tilePrefabPairs;

    /// <summary>
    /// Fetches the prefab associated with a given <c>TileType</c>
    /// </summary>
    /// <param name="tile">The <c>TileType</c> to retrive the prefab for</param>
    /// <returns>A <c>GameObject</c> prefab for the square</returns>
    /// <exception cref="Incorrect prefab assignment">Throws if the number of prefabs assigned to a tile type is not 1</exception>
    public GameObject GetPrefab(TileType tile)
    {
        // Finds all the pairs where the tile type matches the one given
        List<TilePrefabPair> foundPairs = tilePrefabPairs.FindAll(pair => pair.tileType == tile);
        
        // If zero or more than 1 prefab is found for the given tile type, throw an exception.
        if(foundPairs.Count!=1)
        {
            throw new Exception(foundPairs.Count.ToString()+" prefabs found for TileType."+ Enum.GetName(typeof(TileType),tile));
        }

        return foundPairs[0].prefab;
    }

    [System.Serializable]
    struct TilePrefabPair
    {
        public TileType tileType;
        public GameObject prefab;
    }
}