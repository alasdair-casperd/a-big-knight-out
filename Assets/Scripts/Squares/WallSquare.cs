using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A basic wall square which is never passable.
/// </summary>
public class WallSquare : Square
{
    [SerializeField]
    private DynamicSquareGraphics dynamicSquareGraphics;

    public override TileType Type => TileType.Wall;

    public override bool BlocksJump => true;

    // Will always report as passable, if you try to change that you get a warning.
    public override bool IsPassable
    {
        get
        {
            return false;
        }
        protected set
        {
            Debug.LogWarning("Trying to change whether a wall square is passable!");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }
}
