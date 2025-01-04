using UnityEngine;

/// <summary>
/// This gives all of the information necessary to build an entity, it is used by the <c>Level</c> class.
/// </summary>
[System.Serializable]
public class Entity
{
     /*
        Entity Properties
    */

    /// <summary>
    /// The entity's entityType, e.g. Pawn, Rook or Knight
    /// </summary>
    [SerializeField]
    public EntityType Type;

    /// <summary>
    /// An integer used to store the initial state of the entity (probably for storing direction)
    /// </summary>
    public int InitialState;

    /// <summary>
    /// An integer used to store the graphics variant the entity uses
    /// </summary>
    public int GraphicsVariant;

    /*
        Initialisers
    */

    // Minimal initialiser
    public Entity(EntityType type)
    {
        this.Type = type;
        this.InitialState = 0;
        this.GraphicsVariant = 0;
    }

    // Full Initialiser
    public Entity(EntityType type, int initialState, int graphicsVariant)
    {
        this.Type = type;
        this.InitialState = initialState;
        this.GraphicsVariant = graphicsVariant;
    }
}
