using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    /// Lets the square know where it is.
    /// </summary>
    public Vector2Int Position { get; set; }

    /// <summary>
    /// The list of all of the squares that this square is linked to
    /// </summary>
    public List<Square> Links
    {
        get
        {
            return _links;
        }
        set
        {
            if (Type.IsLinkable) _links = value;
            else Debug.Log($"Attempting to assign links to a tile of type '{Type.DisplayName}'");
        }
    }
    private List<Square> _links;

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

    /// <summary>
    /// Update the graphics of the square to match the current state
    /// </summary>
    public virtual void UpdateGraphics()
    {
        return;
    }

    /*
        Electricity Functions and Properties
    */

    // A dictionary whose keys are all the squares that could be sending charge to this square. The
    // dictionary's values indicate and whether each square is sending charge
    public Dictionary<Square, bool?> IncomingCharges;

    // Is the square emitting a charge? Updating this propagates changes through the electrical network
    protected bool OutgoingCharge
    {
        set
        {
            if (!Type.IsConductor) return;
            foreach (var link in Links)
            {
                // TODO: Guard against cycles causing stack overflows
                link.UpdateIncomingCharge(value, this);
            }
        }
    }

    // Is the square receiving any charge?
    protected bool IsReceivingCharge => IncomingCharges.Values.Any(charge => charge ?? false);

    // A function called whenever the charge of this square has been updated
    public virtual void OnChargeChanged() { }

    // A function called by other squares when their charge is updated
    private void UpdateIncomingCharge(bool newCharge, Square sender)
    {
        if (!IncomingCharges.ContainsKey(sender))
        {
            Debug.LogWarning($"Conductive links incorrectly set up for square of type {Type.DisplayName}");
        }

        if (IncomingCharges[sender] != newCharge)
        {
            IncomingCharges[sender] = newCharge;
            OnChargeChanged();
            UpdateOutgoingCharge();
        }
    }

    // Call RecalculateCharge to update the outgoing charge
    public void UpdateOutgoingCharge()
    {
        OutgoingCharge = CalculateCharge();
    }

    // A function to be implemented by each square to control how outgoing charge is determined
    public virtual bool CalculateCharge()
    {
        return false;
    }

    /*

    The following functions may or may not be implemented by a given square. They occur
    in the order that they are written below, and are called by SquareManager.

    */

    /// <summary>
    /// The actions to be performed at the start of the level.
    /// </summary>
    public virtual void OnLevelStart() { }

    /// <summary>
    /// The actions to be performed at the start of the player's turn
    /// </summary>
    public virtual void OnPlayerTurnStart() { }

    /// <summary>
    /// The actions to be performed once the player has input their move
    /// </summary>
    public virtual void OnPlayerMove() { }

    /// <summary>
    /// The actions to be performed immediately when the player lands on this tile
    /// </summary>
    public virtual void OnPlayerLand() { }

    /// <summary>
    /// The actions to be performed immediately when the player leaves the tile
    /// </summary>
    public virtual void OnPlayerLeave() { }

    /// <summary>
    /// The actions to be performed at the start of the Level's turn
    /// </summary>
    public virtual void OnLevelTurn() { }

    /// <summary>
    /// The actions to be performed when an enemy lands on a tile.
    /// </summary>
    public virtual void OnEnemyLand() { }

    /// <summary>
    /// The actions to be performed when an enemy lands on a tile.
    /// </summary>
    public virtual void OnEnemyLand(Enemy enemy) { }

    /// <summary>
    /// The actions to be performed when an enemy leaves a tile.
    /// </summary>
    public virtual void OnEnemyLeave() { }
}
