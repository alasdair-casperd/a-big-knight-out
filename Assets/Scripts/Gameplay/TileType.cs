
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

    public readonly int ID;
    public readonly string DisplayName;

    public readonly List<int> ValidStates;
    public readonly List<int> ValidLinkTargetIDs;

    public readonly List<TileType> ValidLinkTargets
    {
        get
        {
            var targetIDs = ValidLinkTargetIDs;
            return All.Where(type => targetIDs.Contains(type.ID)).ToList();
        }
    }
    
    public readonly bool IsMultiState
    {
        get { return ValidStates.Count > 0; }
    }

    public readonly bool IsLinkable
    {
        get { return ValidLinkTargets.Count > 0; }
    }

    /*
        TileType Properties
    */
    
    public TileType(int id, string displayName, List<int> validStates, List<int > validLinkTargetIDs)
    {
        ID = id;
        DisplayName = displayName;
        ValidStates = validStates;
        ValidLinkTargetIDs = validLinkTargetIDs;
    }  

    public static TileType Floor = new
    (
        id: 0,
        displayName: "Floor",
        validStates: new(),
        validLinkTargetIDs: new()
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
        validLinkTargetIDs: new()
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
        validLinkTargetIDs: new() { 5 }
    );
    
    /*
        All of the game's TileTypes
    */

    public static TileType[] All = {Floor, Wall, FallingFloor, Portal, MovingPlatform, Spikes};

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