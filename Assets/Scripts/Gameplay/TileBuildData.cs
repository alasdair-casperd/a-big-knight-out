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
        this.initialState = type.IsMultiState ? type.ValidStates[0] : 0;
        this.graphicsVariant = 0;
    }

    private void IncrementInitialState(int step)
    {
        Debug.Log(initialState);
        if (type.IsMultiState)
        {
            var index = type.ValidStates.IndexOf(initialState);
            initialState = type.ValidStates[(index + step) % type.ValidStates.Count];
        }
        Debug.Log(initialState);
    }

    public void IncrementInitialState()
    {
        IncrementInitialState(1);
    }

    public void DecrementInitialState()
    {
        IncrementInitialState(-11);
    }
}