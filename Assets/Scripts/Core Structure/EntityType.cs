using UnityEngine;


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

    /*
        TileType Initialiser
    */

    public EntityType(int id, string displayName)
    {
        ID = id;
        DisplayName = displayName;
    }

    public static EntityType Pawn = new
    (
        id: 0,
        displayName: "Pawn"
    );
    public static EntityType Rook = new
    (
        id: 1,
        displayName: "Rook"
    );
    public static EntityType Bishop = new
    (
        id: 2,
        displayName: "Bishop"
    );
    
    // A list of all the tile types
    public static EntityType[] All =
    {
        Pawn,
        Rook,
        Bishop
    };
}
