using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

/// <summary>
/// A square used to launch a specified level
/// </summary>
public class LevelSquare : Square
{
    public override TileType Type => TileType.Level;

    [SerializeField]
    private TextMeshPro levelText;

    private GameManager gameManager;

    /// <summary>
    /// The length of time to pause before transitioning to the level.
    /// </summary>
    public static float pauseTime = 0.5f;

    private void Start()
    {
        var gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.None)[0];
        if (gameManager == null) return;
    }

    private void GetGameManager()
    {
        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.None)[0];
    }

    // Passable only if the level is unlocked
    public override bool IsPassable
    {
        get
        {
            return true;
        }
        protected set { }
    }

    // Sets up the property for graphics variant
    public override int GraphicsVariant { get; set; }

    /// <summary>
    /// When the player lands on the square, transition to the target level. The target level is specified by the state.
    /// </summary>
    public override void OnPlayerLand()
    {
        // Play a success sound
        AudioManager.Play(AudioManager.SoundEffects.success);

        // Transition to the target level
        void transition()
        {
            GetGameManager();

            var targetIndex = State;

            var targetLevel = gameManager.LevelManager.Levels[targetIndex];
            if (targetLevel == null)
            {
                Debug.Log("Error: target level does not exist.");
                return;
            }

            if (gameManager == null) return;
            gameManager.TransitionToLevel(targetLevel);
        }

        // After a delay, transition to the target level
        LeanTween.delayedCall(pauseTime, transition);
    }

    public override void UpdateGraphics()
    {
        var text = State.ToString();
        if (text.Length == 1) text = '0' + text;
        levelText.text = text;
    }
}
