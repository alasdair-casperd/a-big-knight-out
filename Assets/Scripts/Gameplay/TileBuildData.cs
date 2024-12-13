using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This gives all of the information necessary to build a tile, it is used by the <c>Level</c> class.
/// </summary>
[System.Serializable]
public struct TileBuildData
{
    [SerializeField]
    public TileType type;
    public List<Vector2Int> links; // List of location of linked tiles
    public int initialState;      // For tiles with multiple initial states
    public int graphicsVariant;   // For tiles with multiple graphics variants}

    // Minimal initialiser
    public TileBuildData(TileType type)
    {
        this.type = type;
        this.links = new();
        this.initialState = 0;
        this.graphicsVariant = 0;
    }

    /// <summary>
    /// A function to generate a 'deep copy' of the struct
    /// </summary>
    /// <returns></returns>
    public TileBuildData DeepCopy()
    {
        var output = new TileBuildData(type);
        output.links = new(links);
        output.initialState = initialState;
        output.graphicsVariant = graphicsVariant;

        return output;
    }
}