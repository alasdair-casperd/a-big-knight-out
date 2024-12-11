using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A portal square which will link you to a another tile.
/// </summary>
public class PortalSquare : Square
{
    public override TileType Type
    {
        get { return TileType.Portal; }
    }

    public override bool IsLinkable { get { return true; } }
    public override bool IsMultiState { get { return false; } }

    public override List<Square> Links
    {
        get; set;
    }

    // Will always report as passable, if you try to change that you get a warning.
    public override bool IsPassable
    {
        get
        {
            return true;
        }
        protected set
        {
            Debug.LogWarning("Trying to change if portal is passable!");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    /// <summary>
    /// Moves the player when they land on the tile.
    /// </summary>
    public override void OnPlayerLand()
    {
        // Attempt to move knight.
        PlayerController.MoveTo(Links[0].Position, AnimationController.MovementType.Teleport);
    }
}
