using System.Collections.Generic;
using UnityEngine;

public class Level
{
    // A name for the level
    public string Name = "New Level";

    // The player's starting position
    public Vector2Int startPosition = Vector2Int.zero;

    // A dictionary containing all of the level's tiles and their positions
    public Dictionary<Vector2Int, Tile> tiles = new();

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

    private class SerializableLevel
    {
        public Vector2Int startPosition;
        public List<(Vector2Int, Tile)> tiles;
    }
}