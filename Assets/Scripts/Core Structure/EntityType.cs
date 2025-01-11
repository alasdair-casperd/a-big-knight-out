using UnityEditor;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// The information which defines a particular entity type and which is required for level validation (probably)
/// In its current form this is just a glorified enum.
/// </summary>
[System.Serializable]
public struct EntityType
{
    /*
        EntityType Properties
    */

    /// <summary>
    /// An ID for the entity type. This must be unique
    /// </summary>
    public readonly int ID;

    /// <summary>
    /// A name to display in-game to represent this entity type
    /// </summary>
    public readonly string DisplayName;

    /// <summary>
    /// The list of valid states this entity can have, e.g. {0, 1}
    /// </summary>
    public readonly List<int> ValidStates;


    /// <summary>
    /// Can this tile be given initial states?
    /// </summary>
    public readonly bool IsMultiState
    {
        get { return ValidStates.Count > 0; }
    }

    /*
        EntityType Initialiser
    */

    public EntityType(int id, string displayName, List<int> validStates)
    {
        ID = id;
        DisplayName = displayName;
        ValidStates = validStates;
    }

    public static EntityType Pawn = new
    (
        id: 0,
        displayName: "Pawn",
        validStates: new() { 0, 1, 2, 3 }
    );
    public static EntityType Rook = new
    (
        id: 1,
        displayName: "Rook",
        validStates: new() { 0, 1, 2, 3 }

    );
    public static EntityType Bishop = new
    (
        id: 2,
        displayName: "Bishop",
        validStates: new() { 0, 1, 2, 3 }

    );

    // A list of all the tile types
    public static EntityType[] All =
    {
        Pawn,
        Rook,
        Bishop
    };


    /*
        Operator Overloads
    */

    // Equality
    public static bool operator ==(EntityType a, EntityType b)
    {
        return a.ID == b.ID;
    }

    // Inequality
    public static bool operator !=(EntityType a, EntityType b)
    {
        return a.ID != b.ID;
    }

    // Object equality
    public override bool Equals(object obj)
    {
        if (obj is EntityType other)
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
