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

    public override void OnPlayerLand()
    {
        Debug.Log("Now you are thinking");
        Debug.Log("I am here:");
        Debug.Log(Position.ToString());
        Debug.Log("I am paired with a tile at:");
        foreach (var link in Links)
        {
            Debug.Log(link.Position.ToString());
        }

        // Attempt to move knight.
        PlayerController.MoveToVector2(Links[0].Position);
        //Tell the square manager the horse has moved.


    }
}
