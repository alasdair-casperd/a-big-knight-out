using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// An end square which is always passable.
/// </summary>
public class FinishSquare : Square
{
    public override TileType Type =>  TileType.Finish;

    // Will always report as passable, if you try to change that you get a warning.
    public override bool IsPassable
    {
        get
        {
            return true;
        }
        protected set
        {
            Debug.LogWarning("Trying to change whether a finish square is passable!");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    public override void OnPlayerLand()
    {
        AudioManager.Play(AudioManager.SoundEffects.success);
    }
}
