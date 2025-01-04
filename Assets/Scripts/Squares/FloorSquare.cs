using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A basic floor square which is always passable.
/// </summary>
public class FloorSquare : Square
{
    public override TileType Type =>  TileType.Floor;

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
        AudioManager.Play(AudioManager.SoundEffects.thud);
    }
}
