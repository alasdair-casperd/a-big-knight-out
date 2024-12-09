using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A basic floor square which is always passable.
/// </summary>
public class PortalSquare : Square
{
    public override TileType Type
    {
        get { return TileType.Portal; }
    }

    public override bool IsLinkable { get { return true; } }
    public override bool IsMultiState { get { return false; } }

    // Will always report as passable, if you try to change that you get a warning.
    public override bool IsPassable
    {
        get
        {
            return true;
        }
        protected set
        {
            Debug.LogWarning("Trying to change whether a floor square is passable!");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    public override void OnPlayerLand()
    {
        Debug.Log("Clip Clop");
    }
}
