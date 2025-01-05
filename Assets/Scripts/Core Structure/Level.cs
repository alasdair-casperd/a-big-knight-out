using System.Collections.Generic;
using Unity.VisualScripting;
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

    /// <summary>
    /// A dictionary containing all of the level's entities and their positions
    /// </summary>
    public Dictionary<Vector2Int, Entity> Entities;

    /*
        Initialisers
    */

    // Default initialiser
    public Level()
    {
        Name = "New Level";
        StartPosition = Vector2Int.zero;
        Tiles = new();
        Entities = new();
    }

    // Create a level with a single starting square (used in the level editor when making a new level)
    public Level(Vector2Int startPosition)
    {
        Name = "New Level";
        StartPosition = startPosition;
        Tiles = new()
        {
            { startPosition, new Tile(TileType.Floor) }
        };
        Entities = new();
    }

    // Full initialiser
    public Level(string name, Vector2Int startPosition, Dictionary<Vector2Int, Tile> tiles, Dictionary<Vector2Int, Entity> entities)
    {
        Name = name;
        StartPosition = startPosition;
        Tiles = tiles;
        Entities = entities;
    }

    /*
        Validation
    */

    public bool IsValidLevel
    {
        get
        {
            // Check that the start position corresponds to a tile
            if (!Tiles.ContainsKey(StartPosition))
            {
                Debug.LogWarning("No tile found at level's start position");
                return false;
            }

            // Check that the tile at the start position is a valid start location
            var startTile = Tiles[StartPosition];
            if (!startTile.Type.IsValidStartPosition)
            {
                Debug.LogWarning($"Tile at level's start position is not a valid start position. The player cannot start on a tile of type '{startTile.Type.DisplayName}'");
                return false;
            }

            // Check that the entities are placed on valid squares
            foreach (var (position, entity) in Entities)
            {
                // Check the entity is on a square
                if (!Tiles.ContainsKey(position))
                {
                    Debug.LogWarning($"Entity '{entity.Type.DisplayName}' is placed on a tile that does not exist");
                    return false;
                }

                // Check the entity is on a valid start square
                if (!Tiles[position].Type.IsValidStartPosition)
                {
                    Debug.LogWarning($"Entity '{entity.Type.DisplayName}' is placed on a tile that is not a valid start position");
                    return false;
                }
            }

            // Loop over all tiles
            foreach (var (position, tile) in Tiles)
            {
                // Create a string description of the tile's name and position
                string tileDescription = $"A tile of type {tile.Type.DisplayName} at position '({position.x},{position.y})";

                // Check initial state
                if (tile.InitialState != 0 && !tile.Type.ValidStates.Contains(tile.InitialState))
                {
                    Debug.LogWarning(tileDescription + $" has state {tile.InitialState}, which is invalid for this tile type'");
                    return false;
                }

                // Check links
                foreach(var link in tile.Links)
                {
                    // Check for self-links
                    if (link == position)
                    {
                        Debug.LogWarning(tileDescription + $" is linked to itself'");
                        return false;
                    }

                    // Check that there is a tile at the linked location
                    if (!Tiles.ContainsKey(link))
                    {
                        Debug.LogWarning(tileDescription + $" contains a link to a position with no corresponding tile'");
                        return false;
                    }
                    
                    // Check that the link is to a tile of an appropriate type
                    var linkedTile = Tiles[link];
                    if (!tile.Type.ValidLinkTargets.Contains(linkedTile.Type))
                    {
                        Debug.LogWarning(tileDescription + $" contains a link to a position to a tile of type {linkedTile.Type.DisplayName}.'");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}