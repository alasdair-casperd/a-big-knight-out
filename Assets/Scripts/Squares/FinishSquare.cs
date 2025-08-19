using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// An end square which is always passable.
/// </summary>
public class FinishSquare : Square
{
    public override TileType Type => TileType.Finish;

    /// <summary>
    /// The length of time to pause before transitioning away from the level.
    /// </summary>
    public static float gloatingTime = 1.5f;

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
        // Play a success sound
        AudioManager.Play(AudioManager.SoundEffects.success);

        // Transition to the menu level
        void transition()
        {
            var gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.None)[0];
            if (gameManager == null) return;
            gameManager.TransitionToLevel(gameManager.LevelManager.MenuLevel);
        }

        // After a delay, transition back to the menu level
        LeanTween.delayedCall(gloatingTime, transition);
    }
}
