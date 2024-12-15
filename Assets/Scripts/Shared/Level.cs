using System.Collections.Generic;
using UnityEngine;

public class Level
{
    /*
        Level Properties
    */

    /// <summary>
    /// A name for the level
    /// </summary>
    public string Name;

    /// <summary>
    /// The player's starting position
    /// </summary>
    public Vector2Int StartPosition;

    /// <summary>
    /// A dictionary containing all of the level's tiles and their positions
    /// </summary>
    public Dictionary<Vector2Int, Tile> Tiles;

    /*
        Initialisers
    */

    // Default initialiser
    public Level()
    {
        Name = "New Level";
        StartPosition = Vector2Int.zero;
        Tiles = new();
    }

    // Full initialiser
    public Level(string name, Vector2Int startPosition, Dictionary<Vector2Int, Tile> tiles)
    {
        Name = name;
        StartPosition = startPosition;
        Tiles = tiles;
    }

    /*
        Validation
    */

    public void ValidateLevel()
    {
        // // Loops over all the tiles
        // foreach (TilePositionPair tilePosPair in tiles)
        // {
        //     // Finds all tiles with the position of this tile, if there is more than one, raise an error.
        //     List<TilePositionPair> tilesAtPosition = tiles.FindAll(tile => tile.position == tilePosPair.position);
        //     if(tilesAtPosition.Count !=1)
        //     {
        //         throw new Exception("Multiple tiles found at position "+ tilePosPair.position.ToString());
        //     }

        //     // TODO: Reinstate validation below, previously in LevelBuilder

        //     // Loops over all of the tiles
        //     // foreach (var (position, tile) in workingLevel.tiles)
        //     // {
        //     //     // Checks if the tile is trying to link to itself (bad)
        //     //     if (tile.links.Contains(position))
        //     //     {
        //     //         Debug.LogWarning("Trying to link a tile to itself at position " + position.ToString());
        //     //     }

        //     //     // Checks if there are links registered to an unlinkable tile
        //     //     if (tile.links.Count != 0 && !squares[position].IsLinkable)
        //     //     {
        //     //         Debug.LogWarning("Trying to link an unlinkable tile at position " + position.ToString());
        //     //     }

        //     //     // Checks if the link goes to a location with no tile created
        //     //     foreach (Vector2Int link in tile.links)
        //     //     {
        //     //         if (!squares.Keys.Contains(link))
        //     //         {
        //     //             throw new Exception("Trying to create a link to a tile that does not exist from " + position.ToString() + " to " + link.ToString());
        //     //         }
        //     //     }
        //     // }
        // }
    }


}