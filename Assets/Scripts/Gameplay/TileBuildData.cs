using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This gives all of the information necessary to build a tile, it is used by the <c>Level</c> class.
/// </summary>
[System.Serializable]
public struct TileBuildData
{
    public TileType type;
    public List<Vector2Int> links; // List of location of linked tiles
    public int initialState;      // For tiles with multiple initial states
    public int graphicsVariant;   // For tiles with multiple graphics variants}
}



/// <summary>
/// A label to identify the type of tile
/// </summary>
public enum TileType
{
    Floor,
    Wall,
    Spikes,
    Portal,
    Spike,
    MovingPlatform,

    FallingFloor
}