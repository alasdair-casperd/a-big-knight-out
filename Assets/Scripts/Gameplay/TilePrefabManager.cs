using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "TilePrefabManager", menuName = "Scriptable Objects/TilePrefabManager")]
public class TilePrefabManager : ScriptableObject
{
    [SerializeField] // Allows the private field to be shown in the inspector
    List<TilePrefabPair> tilePrefabPairs;

    /// <summary>
    /// Fetches the prefab for a given <c>TileType</c>
    /// </summary>
    /// <param name="tileType">The <c>TileType</c> to be looked up</param>
    /// <returns></returns>
    public GameObject GetPrefab(TileType tileType)
    {
        int tileID = tileType.id;
        GameObject prefab = null;

        // Loops over all the assigned pairs of tileTypeEnum and prefab 
        foreach (TilePrefabPair tilePrefabPair in tilePrefabPairs)
        {
            // Checks if the ID of the tile given matches the ID of the tile enum
            // Scott don't look I'm casting
            if((int)tilePrefabPair.tile == tileID)
            {
                // If it does then this is the prefab we want
                prefab = tilePrefabPair.tilePrefab;
            }
        }

        // If it hasn't found a prefab then throw an error
        if(prefab == null)
        {
            Debug.LogError("No prefab found for tile type "+tileType.name);
        }

        return prefab;
    }

    // Forgive me father for I have enumed (this is a messy hack but only this class needs to worry about it)
    // The name is Scott's fault

    // The integer MUST MATCH the id of the corresponding TileType
    private enum TileTypeEnum
    {
        Floor = 1,
        Wall = 2
    }

    // Pairs which match TileTypeEnum with a prefab GameObject
    [System.Serializable] // To allow it to be displayed in inspector
    struct TilePrefabPair
    {
        [SerializeField]
        public TileTypeEnum tile;
        [SerializeField]
        public GameObject tilePrefab;
    }
}