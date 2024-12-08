using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Stores all of the data required to build a level
/// </summary>
[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{

    // The player's starting position
    public Vector2Int startPos;

    // A list containing all of the level's tiles.
    public List<TilePositionPair> tiles;

    /// <summary>
    /// Validates that the level is valid, e.g. there are no duplicate positions.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void ValidateLevel()
    {
        // Loops over all the tiles
        foreach (TilePositionPair tilePosPair in tiles)
        {
            // Finds all tiles with the position of this tile, if there is more than one, raise an error.
            List<TilePositionPair> tilesAtPosition = tiles.FindAll(tile => tile.position == tilePosPair.position);
            if(tilesAtPosition.Count !=1)
            {
                throw new Exception("Multiple tiles found at position "+ tilePosPair.position.ToString());
            }
        }
    }
}

/// <summary>
/// Stores the position of a tile as well as its tile build data
/// </summary>
[System.Serializable]
public struct TilePositionPair
{
    public Vector2Int position;
    public TileBuildData tile;
}
