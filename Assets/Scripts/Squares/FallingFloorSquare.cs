using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// A square that after you land on, will disappear.
/// </summary>
public class FallingFloorSquare : Square
{

    public override TileType Type => TileType.FallingFloor;

    // Is only passable before it is landed on.
    public override bool IsPassable
    {
        get
        {
            return !hasFallen;
        }
        protected set
        {
            Debug.LogWarning("Attempting to manually change whether a falling floor square is passable");
        }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    private bool hasFallen;

    [Header("Graphics")]
    public FallingFloorSquareGraphics Graphics;

    public float FallHeight = 3;

    public float FallDuration = 0.5f;

    /// <summary>
    /// When the platform lands
    /// </summary>
    public override void OnPlayerLand()
    {   
        // Play a sound effect
        AudioManager.Play(AudioManager.SoundEffects.thud);

        // Show a subtle crack animation
        Graphics.Crack();
    }


    /// <summary>
    /// On the player leave, the square will fall away and will no longer be passable.
    /// </summary>
    public override void OnPlayerLeave()
    {
        Fall();
    }

    /// <summary>
    /// On the enemy leave, the square will fall away and will no longer be passable.
    /// </summary>
    public override void OnEnemyLeave()
    {
        Fall();
    }

    /// <summary>
    /// Fall away and become impassable.
    /// </summary>
    private void Fall()
    {
        // Sets the state ready to fall when the player leaves.
        hasFallen = true;

        // Fall away
        Graphics.Fall();

        // Play a sound effect
        AudioManager.Play(AudioManager.SoundEffects.fallingPlatform);
    }
}