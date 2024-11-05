using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A structure to hold data about types of tile. Can be used like an enum, for example
/// <c>TileType.floor</c>
/// and holds additional data, for example
/// <c>TileType.floor.hasState</c>
/// </summary>
public struct TileType
{
    // The unique id of the tile type
    public int id;
    // Whether the tile type has multiple states
    public bool hasState;
    // Whether the tile type can be linked to other tiles
    public bool linkable;

    public string name;

    /// <summary>
    /// A bog-standard floor tile
    /// </summary>
    public static TileType floor = new()
    {
        id = 1,
        name = "floor",
        hasState = false,
        linkable = false
    };
    /// <summary>
    /// A bog-standard wall tile
    /// </summary>
    public static TileType wall = new()
    {
        id = 2,
        name = "wall",
        hasState = false,
        linkable = false
    };
}