using UnityEngine;
using System.Collections.Generic;

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
    public GameObject Graphics;

    public float FallHeight = 3;

    public float FallDuration = 0.5f;

    /// <summary>
    /// When the platform lands
    /// </summary>
    public override void OnPlayerLand()
    {
        // Play a sound effect
        AudioManager.Play(AudioManager.SoundEffects.thud);
    }


    /// <summary>
    /// On the player leave, the square will fall away and will no longer be passable.
    /// </summary>
    public override void OnPlayerLeave()
    {
        // Sets the state ready to fall when the player leaves.
        hasFallen = true;

        // Fall away
        Vector3 initialPosition = transform.position;
        Vector3 initialScale = transform.localScale;
        LeanTween.value(Graphics, 0, 1, FallDuration)
            .setOnUpdate(t =>
            {
                transform.position = initialPosition - Vector3.up * FallHeight * t;
                transform.localScale = initialScale * (1 - t);
            })
            .setOnComplete(() => Destroy(Graphics));

        // Play a sound effect
        AudioManager.Play(AudioManager.SoundEffects.fallingPlatform);
    }

    /// <summary>
    /// On the enemy leave, the square will fall away and will no longer be passable.
    /// </summary>
    public override void OnEnemyLeave()
    {
        // Sets the state ready to fall when the player leaves.
        hasFallen = true;

        // Fall away
        Vector3 initialPosition = transform.position;
        Vector3 initialScale = transform.localScale;
        LeanTween.value(Graphics, 0, 1, FallDuration)
            .setOnUpdate(t =>
            {
                transform.position = initialPosition - Vector3.up * FallHeight * t;
                transform.localScale = initialScale * (1 - t);
            })
            .setOnComplete(() => Destroy(Graphics));

        // Play a sound effect
        AudioManager.Play(AudioManager.SoundEffects.fallingPlatform);
    }
}