
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public struct TileType
{
    /*
        TileType Properties
    */

    /// <summary>
    /// An ID for the tile type. This must be unique
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// A name to display in-game to represent this tile type
    /// </summary>
    public readonly string DisplayName;

    /// <summary>
    /// The list of valid states this tile can have, e.g. {0, 1}
    /// </summary>
    public readonly List<int> ValidStates;

    /// <summary>
    /// The list of the ID's of the tile types that this tile can be linked to
    /// </summary>
    private readonly List<int> ValidLinkTargetIDs;

    /// <summary>
    /// Can a tile of this type be used as the start location for the player?
    /// </summary>
    public readonly bool IsValidStartPosition;

    /// <summary>
    /// Should the links of this tile be considered as electrical links?
    /// </summary>
    public readonly bool IsConductor;

    /// <summary>
    /// The list of tile types that this tile can be linked to
    /// </summary>
    public readonly List<TileType> ValidLinkTargets
    {
        get
        {
            var targetIDs = ValidLinkTargetIDs;
            return All.Where(type => targetIDs.Contains(type.ID)).ToList();
        }
    }

    /// <summary>
    /// Can this tile be given initial states?
    /// </summary>
    public readonly bool IsMultiState
    {
        get { return ValidStates.Count > 0; }
    }

    /// <summary>
    /// Can this tile be linked to other tiles?
    /// </summary>
    public readonly bool IsLinkable
    {
        get { return ValidLinkTargets.Count > 0; }
    }

    /*
        TileType Initialiser
    */

    public TileType(int id, string displayName, List<int> validStates, List<int> validLinkTargetIDs, bool isValidStartPosition = false, bool isConductor = false)
    {
        ID = id;
        DisplayName = displayName;
        ValidStates = validStates;
        ValidLinkTargetIDs = validLinkTargetIDs;
        IsValidStartPosition = isValidStartPosition;
        IsConductor = isConductor;
    }

    /*
        All of the game's TileTypes
        (When adding a new one, make sure it add it to the list TileType.All)
    */

    public static TileType Floor = new
    (
        id: 0,
        displayName: "Floor",
        validStates: new(),
        validLinkTargetIDs: new(),
        isValidStartPosition: true
    );

    public static TileType Wall = new
    (
        id: 1,
        displayName: "Wall",
        validStates: new(),
        validLinkTargetIDs: new()
    );

    public static TileType FallingFloor = new
    (
        id: 2,
        displayName: "Falling Platform",
        validStates: new(),
        validLinkTargetIDs: new(),
        isValidStartPosition: true
    );

    public static TileType Portal = new
    (
        id: 3,
        displayName: "Portal",
        validStates: new(),
        validLinkTargetIDs: new() { 0, 3, 5 }
    );

    public static TileType MovingPlatform = new
    (
        id: 4,
        displayName: "Moving Platform",
        validStates: new() { 0, 1 },
        validLinkTargetIDs: new() { 4 }
    );

    public static TileType Spikes = new
    (
        id: 5,
        displayName: "Spikes",
        validStates: new() { 0, 1 },
        validLinkTargetIDs: new() { 5 },
        isValidStartPosition: true
    );

    public static TileType SpikeUp = new
    (
        id: 6,
        displayName: "Spike Up",
        validStates: new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
        validLinkTargetIDs: new()
    );

    public static TileType SpikeDown = new
    (
        id: 7,
        displayName: "Spike Down",
        validStates: new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
        validLinkTargetIDs: new()
    );

    public static TileType Finish = new
    (
        id: 8,
        displayName: "FinishTile",
        validStates: new(),
        validLinkTargetIDs: new(),
        isValidStartPosition: false
    );

    public static TileType ColourFlip = new
    (
        id: 9,
        displayName: "ColourFlip",
        validStates: new() { 0, 1 },
        validLinkTargetIDs: new(),
        isValidStartPosition: true
    );

    public static TileType Button = new
    (
        id: 10,
        displayName: "Button",
        validStates: new(),
        validLinkTargetIDs: new() { 5, 6, 7, 11, 12, 13, 14 },
        isConductor: true
    );

    public static TileType Switch = new
    (
        id: 11,
        displayName: "Switch",
        validStates: new() { 0, 1 },
        validLinkTargetIDs: new() { 5, 6, 7, 11, 12, 13, 14 },
        isConductor: true
    );

    public static TileType AndGate = new
    (
        id: 12,
        displayName: "AND Gate",
        validStates: new(),
        validLinkTargetIDs: new() { 5, 6, 7, 11, 12, 13, 14 },
        isConductor: true
    );

    public static TileType OrGate = new
    (
        id: 13,
        displayName: "OR Gate",
        validStates: new(),
        validLinkTargetIDs: new() { 5, 6, 7, 11, 12, 13, 14 },
        isConductor: true
    );

    public static TileType NotGate = new
    (
        id: 14,
        displayName: "NOT Gate",
        validStates: new(),
        validLinkTargetIDs: new() { 5, 6, 7, 11, 12, 13, 14 },
        isConductor: true
    );

    // A list of all the tile types
    public static TileType[] All =
    {
        Floor,
        Wall,
        FallingFloor,
        Portal,
        MovingPlatform,
        Spikes,
        SpikeUp,
        SpikeDown,
        ColourFlip,
        Finish,
        Button,
        Switch,
        AndGate,
        OrGate,
        NotGate
    };

    /*
        Operator Overloads
    */

    // Equality
    public static bool operator ==(TileType a, TileType b)
    {
        return a.ID == b.ID;
    }

    // Inequality
    public static bool operator !=(TileType a, TileType b)
    {
        return a.ID != b.ID;
    }

    // Object equality
    public override bool Equals(object obj)
    {
        if (obj is TileType other)
        {
            return ID == other.ID;
        }
        return false;
    }

    // Hash code
    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}