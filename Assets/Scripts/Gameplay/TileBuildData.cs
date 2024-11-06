using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This gives all of the information necessary to build a tile, it is used by the <c>Level</c> class.
/// </summary>
public struct TileBuildData
{
    public TileType Type{get;set;}
    public List<Vector2Int> Links{get;set;} // List of location of linked tiles
    public int InitialState{get;set;}       // For tiles with multiple initial states
    public int GraphicsVariant{get;set;}    // For tiles with multiple graphics variants}
}



/// <summary>
/// A label to identify the type of tile
/// </summary>
public enum TileType
{
    Floor,
    Wall,
    Spikes
}