using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// This gives all of the information necessary to build a tile, it is used by the <c>Level</c> class.
/// </summary>
[System.Serializable]
public struct Tile
{
    /*
        Tile Properties
    */

    /// <summary>
    /// The tile's tileType, e.g. Floor, Portal or Spikes
    /// </summary>
    [SerializeField]
    public TileType Type;

    /// <summary>
    /// An integer used to store the initial state of the tile
    /// </summary>
    public int InitialState;

    /// <summary>
    /// An integer used to store the graphics variant the tile uses
    /// </summary>
    public int GraphicsVariant;

    /// <summary>
    /// A list of the locations of the tiles this tile is linked to
    /// </summary>
    public List<Vector2Int> Links;

    /*
        Initialisers
    */

    // Minimal initialiser
    public Tile(TileType type)
    {
        this.Type = type;
        this.InitialState = type.IsMultiState ? type.ValidStates[0] : 0;
        this.GraphicsVariant = 0;
        this.Links = new();
    }

    // Full Initialiser
    public Tile(TileType type, int initialState, int graphicsVariant, List<Vector2Int> links)
    {
        this.Type = type;
        this.InitialState = initialState;
        this.GraphicsVariant = graphicsVariant;
        this.Links = links;
    }

    /*
        Functions for use in the Level Editor
    */

    // Increase the initial state by the specified step
    private void IncrementInitialState(int step)
    {
        if (Type.IsMultiState)
        {
            var index = Type.ValidStates.IndexOf(InitialState);

            index += step;

            while(index < 0)
            {
               index += Type.ValidStates.Count;
            }

            while(index >= Type.ValidStates.Count)
            {
               index -= Type.ValidStates.Count;
            }

            InitialState = Type.ValidStates[index];
        }
    }

    // Increase the initial state by 1
    public void IncrementInitialState()
    {
        IncrementInitialState(1);
    }

    // Decrease the initial state by 1
    public void DecrementInitialState()
    {
        IncrementInitialState(-1);
    }
}