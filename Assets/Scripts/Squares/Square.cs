using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The base class for all squares.
/// </summary>
public abstract class Square : MonoBehaviour
{
    /// <summary>
    /// The type of tile.
    /// </summary>
    public abstract TileType Type { get; }

    /// <summary>
    /// Lets the tile know where it is.
    /// </summary>
    public Vector2Int Position { get; set; }


    /// <summary>
    /// The list of all of the squares that this square is linked to, if not implemented (e.g. for floor), it will raise a warning.
    /// </summary>
    public virtual List<Square> Links
    {
        get
        {
            Debug.LogWarning("Links not implemented for tile type " + Type.DisplayName);
            return new List<Square>();
        }
        set
        {
            Debug.LogWarning("Links not implemented for tile type " + Type.DisplayName);
        }
    }

    /// <summary>
    /// The state of this square, if not implemented (e.g. for floor), it will raise a warning.
    /// </summary>
    public int State
    {
        get
        {
            return _state;
        }
        set
        {
            if (Type.ValidStates.Contains(value)) _state = value;
            else Debug.Log($"Attempting to assign a state of {value} to a tile of type '{Type.DisplayName}'");
        }
    }
    private int _state;

    /// <summary>
    /// The graphics variant to use for this square.
    /// </summary>
    public abstract int GraphicsVariant { get; set; }

    /// <summary>
    /// Whether the tile is currently passable.
    /// </summary>
    public abstract bool IsPassable { get; protected set; }

    /// <summary>
    /// Allows access to the player controller script.
    /// </summary>
    public PlayerController PlayerController { get; set; }

    /// <summary>
    /// Am attached indicator used to show where the player can move
    /// </summary>
    [HideInInspector]
    public UI.ValidMoveIndicator validMoveIndicator;

    /// <summary>
    /// Show whether or not this square constitutes a valid move for the player
    /// </summary>
    /// <param name="isValid">Can the player move to this square?</param>
    public virtual void IndicateMoveValidity(bool isValid)
    {
        if (isValid) validMoveIndicator.Show();
        else validMoveIndicator.Hide();
    }

    /*

    The following functions may or may not be implemented by a given square. They occur
    in the order that they are written below, and are called by SquareManager.

    */

    /// <summary>
    /// The actions to be perfomed at the start of the player's turn
    /// </summary>
    public virtual void OnPlayerTurnStart()
    {
        return;
    }

    /// <summary>
    /// The actions to be performed once the player has input their move
    /// </summary>
    public virtual void OnPlayerMove()
    {
        return;
    }

    /// <summary>
    /// The actions to be performed immediately when the player lands on this tile
    /// </summary>
    public virtual void OnPlayerLand()
    {
        return;
    }

    /// <summary>
    /// The actions to be performed immediately when the player leaves the tile
    /// </summary>
    public virtual void OnPlayerLeave()
    {
        return;
    }

    /// <summary>
    /// The actions to be performed at the start of the Level's turn
    /// </summary>
    public virtual void OnLevelTurn()
    {
        return;
    }

    /// <summary>
    /// The actions to be performed at the start of the level.
    /// </summary>
    public virtual void OnLevelStart()
    {
        return;
    }
}
