
using System;
using System.Collections.Generic;
using System.Linq;


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
        TileType Initialisers
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
        Utility Properties
    */

    // Property to generate all possible spike states
    // This is horrible, and will be removed as soon as enough custom tools have been created in the level editor
    private static List<int> _validSpikeStates;
    private static List<int> ValidSpikeStates
    {   
        get
        {
            if (_validSpikeStates != null) return _validSpikeStates;            
            List<int> numbers = new List<int>();
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                if (current > 2222222)
                {
                    continue;
                }
                numbers.Add(current);
                queue.Enqueue(current * 10 + 1);
                queue.Enqueue(current * 10 + 2);
            }
            
            _validSpikeStates = numbers;
            return _validSpikeStates;
        }
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

    public static TileType Spikes = new
    (
        id: 5,
        displayName: "Spikes",
        validStates: ValidSpikeStates,
        validLinkTargetIDs: new(),
        isValidStartPosition: true
    );

    public static TileType Track = new
    (
        id: 6,
        displayName: "Track",
        validStates: new() {0,1},
        validLinkTargetIDs: new(),
        isConductor: false
    );

    public static TileType Finish = new
    (
        id: 8,
        displayName: "Finish Tile",
        validStates: new(),
        validLinkTargetIDs: new(),
        isValidStartPosition: false
    );

    public static TileType ColourFlip = new
    (
        id: 9,
        displayName: "Colour Flip Tile",
        validStates: new() { 0, 1 },
        validLinkTargetIDs: new(),
        isValidStartPosition: true
    );

    public static TileType Button = new
    (
        id: 10,
        displayName: "Button",
        validStates: new(),
        validLinkTargetIDs: new() { 5, 11, 12, 13, 14 },
        isConductor: true
    );

    public static TileType Switch = new
    (
        id: 11,
        displayName: "Switch",
        validStates: new() { 0, 1 },
        validLinkTargetIDs: new() { 5, 11, 12, 13, 14 },
        isConductor: true
    );

    public static TileType AndGate = new
    (
        id: 12,
        displayName: "AND Gate",
        validStates: new(),
        validLinkTargetIDs: new() { 5, 11, 12, 13, 14 },
        isConductor: true
    );

    public static TileType OrGate = new
    (
        id: 13,
        displayName: "OR Gate",
        validStates: new(),
        validLinkTargetIDs: new() { 5, 11, 12, 13, 14 },
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

    // ^^ Please add IDs 6 and 7 (formerly spike up and spike down) before using 15, 16 etc. tysm

    // A list of all the tile types
    public static readonly TileType[] All =
    {
        Floor,
        Wall,
        FallingFloor,
        Portal,
        Spikes,
        ColourFlip,
        Finish,
        Button,
        Switch,
        AndGate,
        OrGate,
        NotGate,
        Track
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